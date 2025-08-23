from operator import truediv
from typing import Callable, Union, Dict, Set

from BaseClasses import MultiWorld, CollectionState
from ..generic.Rules import add_rule, set_rule
from .Locations import location_table
from .Options import PVZFOptions
from .Regions import connect_regions, SRB2Zones
#from .Items import character_item_data_table
#from .Items import tools_item_data_table
from dataclasses import dataclass


@dataclass
class PlantData:
    """Plant data for creating the rules lambda"""
    name: str
    power: int
    pool_power: int
    roof_power:int
    required_plants: []
    special_tags: [] #doesnt_freeze, no_pool, targets_air, cherry_immune, torchwood_usable, applies_cryo, removes_cryo, no_roof, from_fertilizer, pool_only
    def __init__(self, name:str, power:int, pool_power:int,roof_power:int,required_plants:[],special_tags:[]):
       self.name =  name
       self.power =  power
       self.pool_power =  pool_power
       self.roof_power =  roof_power
       self.required_plants =  required_plants
       self.special_tags =  special_tags


def shuffle_dict_keys(world, dictionary: dict) -> dict:
    keys = list(dictionary.keys())
    values = list(dictionary.values())
    world.random.shuffle(keys)
    return dict(zip(keys, values))

def fix_reg(entrance_map: Dict[SRB2Zones, str], entrance: SRB2Zones, invalid_regions: Set[str],
            swapdict: Dict[SRB2Zones, str], world):
    if entrance_map[entrance] in invalid_regions: # Unlucky :C
        replacement_regions = [(rand_entrance, rand_region) for rand_entrance, rand_region in swapdict.items()
                               if rand_region not in invalid_regions]
        rand_entrance, rand_region = world.random.choice(replacement_regions)
        old_dest = entrance_map[entrance]
        entrance_map[entrance], entrance_map[rand_entrance] = rand_region, old_dest
        swapdict[entrance], swapdict[rand_entrance] = rand_region, old_dest
    swapdict.pop(entrance)#i dont know what this does and im too scared to touch it

def set_rules(world, options: PVZFOptions, player: int, area_connections: dict, move_rando_bitvec: int):

    # Destination Format: LVL | AREA with LVL = LEVEL_x, AREA = Area as used in sm64 code
    # Cast to int to not rely on availability of SM64Levels enum. Will cause crash in MultiServer otherwise
    def can_beat_all_last_levels(state: CollectionState):
        return state.can_reach("Day: Level 9 (1)", "Location", player) and state.can_reach("Night: Level 18 (1)", "Location", player) and state.can_reach("Pool: Level 27 (1)", "Location", player) and state.can_reach("Fog: Level 36 (1)", "Location", player) and state.can_reach("Roof: Level 45 (1)", "Location", player) and state.can_reach("Snow: Level 9 (1)", "Location", player)


    def can_beat_power_level(state: CollectionState, level_strength,  modifier_flags):
        #Breaks when it finds the highest possible plant sorted by power
        free_slots= 4#-1 for sunflower
        possible_plants = []
        final_plants = []
        #check for required plants for things like balloon, scuba, cherry newspaper, sunflower, lily pad, pot to take away free_slots
        # if hard requirements arent found, immediately return false (balloon/scuba)
        if "cherry_newspaper" in modifier_flags:
            if state.has("Cherry Bomb",player) and state.has("Wall-nut",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Wall-nut")
            elif state.has("Cherry Bomb",player) and state.has("Pumpkin",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Pumpkin")
            elif state.has("Melon-pult",player) and state.has("Umbrella Leaf",player):
                final_plants.append("Melon-pult")
                final_plants.append("Umbrella Leaf")
            else:
                return False

        if "balloon" in modifier_flags: #cob cannon CAN target balloons
            #return false if no cactus, blover, cattail, cattail girl
            if "tough_balloon" in modifier_flags:
                #check if any available fusions can deal with zepplins
                pass #TODO this
            else:
                if state.has("Sniper Pea",player) and "odyssey" in modifier_flags:
                    final_plants.append("Sniper Pea")
                elif state.has("Lily Pad", player) and state.has("Fertilizer", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                elif state.has("Cactus",player):
                    final_plants.append("Cactus")
                elif state.has("Blover",player):
                    final_plants.append("Blover")
                elif state.has("Cattail Girl",player) and "water" in modifier_flags:
                    final_plants.append("Cattail Girl")
                elif state.has("Lily Pad", player) and state.has("Cattail", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                    final_plants.append("Cattail")
                else:
                    return False

        if "furlings" in modifier_flags:
            if state.has("Wall-nut", player):
                final_plants.append("Wall-nut")
            elif state.has("Saw-me-not", player):
                final_plants.append("Saw-me-not")
            elif state.has("Pumpkin", player):
                final_plants.append("Pumpkin")
            elif state.has("Bamblock", player):
                final_plants.append("Bamblock")
            else:
                return False
            #if night then puffshroom

        if "digger" in modifier_flags:
            if "water" in modifier_flags:
                if state.has("Lily Pad", player) and state.has("Fertilizer", player):
                    final_plants.append("Lily Pad")
                elif state.has("Cattail Girl", player):
                    final_plants.append("Cattail Girl")
                elif state.has("Lily Pad", player) and state.has("Cattail", player):
                    final_plants.append("Lily Pad")
                    final_plants.append("Cattail")
            elif state.has("Sniper Pea", player) and "odyssey" in modifier_flags:
                final_plants.append("Sniper Pea")
            elif state.has("Swordmaster Starfruit", player):
                final_plants.append("Swordmaster Starfruit")
            elif state.has("Peashooter", player):
                final_plants.append("Peashooter")
            elif state.has("Starfruit", player):
                final_plants.append("Starfruit")
            elif state.has("Fume-shroom", player) and state.has("Fertilizer", player):
                final_plants.append("Fume-shroom")
            elif state.has("Magnet-shroom", player):
                final_plants.append("Magnet-shroom")
            elif state.has("Fume-shroom", player) and state.has("Gloom-shroom", player):
                final_plants.append("Fume-shroom")
                final_plants.append("Gloom-shroom")
            else:
                return False
            #maybe cob cannon, gold cabbage, gold melon, potato mine/ doom shroom

            if "mecha_nut" in modifier_flags:
                if state.has("Umbrella Leaf",player):
                    final_plants.append("Umbrella Leaf")
                else:
                    return False#maybe require jalapeno and ice-shroom?

        while True:
            if state.has("Burger Blaster", player) and state.has("Cactus", player) and state.has("Melon-pult", player) and state.has("Ice-shroom", player) and state.has("Plant Gloves", player):  # with sunflower
                possible_plants.append(PlantData("Burger Blaster", 2000, 0, 1000, ["Burger Blaster", "Cactus","Melon-pult","Ice-shroom"], ["straight_shooter"]))
            if state.has("Burger Blaster", player) and state.has("Cactus", player) and state.has("Melon-pult", player):  # with sunflower
                possible_plants.append(PlantData("Burger Blaster", 1800, 0, 900, ["Burger Blaster", "Cactus", "Melon-pult"],["straight_shooter"]))
            if state.has("Icetip Lily", player):
                possible_plants.append(PlantData("Icetip Lily", 1800, 1500, 700, ["Icetip Lily"], ["applies_cryo"]))
            if state.has("Scaredy-shroom", player) and state.has("Fume-shroom", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Charm-shroom", 1500, 0, 1500, ["Scaredy-shroom", "Fume-shroom","Hypno-shroom"], ["straight_shooter"]))
            if state.has("Sniper Pea", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Sniper Pea", 1500, 1500, 1500, ["Sniper Pea"], []))
            if state.has("Starfruit", player) and state.has("Plantern", player) and state.has("Magnet-shroom", player):
                possible_plants.append(PlantData("Stardrop", 1500, 1500, 900, ["Starfruit", "Plantern","Magnet-shroom"], []))
            if state.has("Cabbage-pult", player) and state.has("Kernel-pult", player) and state.has("Melon-pult", player):
                possible_plants.append(PlantData("Swordmaster Starfruit", 1300, 0, 600, ["Swordmaster Starfruit"], []))
            if state.has("Swordmaster Starfruit", player):
                possible_plants.append(PlantData("Swordmaster Starfruit", 1200, 1200, 600, ["Swordmaster Starfruit"], []))
            if state.has("Melon-pult", player) and state.has("Kernel-pult", player) and state.has("Jicamagic", player):
                possible_plants.append(PlantData("Melon Mortar", 1200, 1200, 1200, ["Melon-pult","Kernel-pult","Jicamagic"], ["scuba_no_lilypad","doesnt_freeze"]))
            if state.has("Amp-nion", player):
                possible_plants.append(PlantData("Ampnion", 1200, 1200, 1200, ["Amp-nion"], []))
            if state.has("Saw-me-not", player):
                possible_plants.append(PlantData("Twin Saw-me-not", 1200, 1200, 1200, ["Saw-me-not"], []))
            if state.has("Kernel-pult", player) and state.has("Marigold", player):
                possible_plants.append(PlantData("Golden Kernel", 1200, 1200, 1200, ["Kernel-pult","Marigold"], []))
            if state.has("Fume-shroom", player) and state.has("Doom-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Doomspike-shroom", 1200, 0, 800, ["Fume-shroom","Doom-shroom","Ice-shroom"], ["applies_cryo"]))
            if state.has("Kernel-pult", player) and (state.has("Cob Cannon", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Cob Cannon", 1000, 1000, 1000, ["Kernel-pult", "Cob Cannon"],["from_fertilizer","scuba_no_lilypad","doesnt_freeze"]))
            if state.has("Peashooter", player) and state.has("Threepeater", player) and state.has("Jicamagic", player):
                possible_plants.append(PlantData("Titan Pea Turret", 1000, 0, 500, ["Peashooter", "Threepeater","Jicamagic"], ["torchwood_usable","straight_shooter","doesnt_freeze"]))
            if state.has("Scaredy-shroom", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Trippy-shroom", 1000, 0, 500, ["Scaredy-shroom", "Hypno-shroom"], ["straight_shooter"]))
            if state.has("Starfruit", player) and state.has("Plantern", player):
                possible_plants.append(PlantData("Starglow", 1000, 1000, 500, ["Starfruit", "Plantern"], []))
            if state.has("Spruce Ballista", player) and state.has("Aloe Aqua", player) and (state.has("Spruce Sharpshooter", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Atlantis Ballista", 1000, 0, 500, ["Spruce Sharpshooter","Aloe Aqua","Spruce Ballista"], ["no_pool","from_fertilizer","applies_cryo","doesnt_freeze"]))
            if state.has("Kernel-pult", player) and state.has("Melon-pult", player):
                possible_plants.append(PlantData("Corn-pult", 1000, 0, 1000, ["Kernel-pult", "Melon-pult"], []))
            if state.has("Melon-pult", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Garlic-pult", 900, 0, 900, ["Melon-pult", "Garlic"], []))
            if state.has("Melon-pult", player) and state.has("Jalapeno", player):
                possible_plants.append(PlantData("Summer Melon", 900, 0, 900, ["Melon-pult", "Jalapeno"], ["removes_cryo"]))
            if state.has("Melon-pult", player) and state.has("Marigold", player):
                possible_plants.append(PlantData("Golden Melon", 900, 450, 900, ["Melon-pult","Marigold"], ["scuba_no_lilypad"]))
            if state.has("Cactus", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Doom Cactus", 900, 100, 300, ["Cactus", "Doom-shroom"], ["straight_shooter"]))
            if state.has("Peashooter", player) and state.has("Puff-shroom", player):
                possible_plants.append(PlantData("Pea-shroom", 900, 0, 300, ["Peashooter","Puff-shroom"], ["torchwood_usable","straight_shooter"]))
            if state.has("Cabbage-pult", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Garbage-pult", 900, 0, 900, ["Cabbage-pult","Garlic"], []))
            if state.has("Melon-pult", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Winter Melon", 800, 0, 800, ["Melon-pult", "Ice-shroom"], ["applies_cryo"]))
            if state.has("Kernel-pult", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Clove-pult", 800, 0, 800, ["Kernel-pult","Garlic"], []))
            if state.has("Melon-pult", player) and state.has("Fume-shroom", player):
                possible_plants.append(PlantData("Spring Melon", 800, 0, 800, ["Melon-pult","Fume-shroom"], []))
            if state.has("Peashooter", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Gatling Snow", 800, 0, 400, ["Peashooter", "Ice-shroom"], ["applies_cryo","straight_shooter"]))
            if state.has("Peashooter", player) and state.has("Cherry Bomb", player):
                possible_plants.append(PlantData("Cherry Gatling", 800, 0, 400, ["Peashooter","Cherry Bomb"], ["straight_shooter"]))
            if state.has("Threepeater", player) and state.has("Squash", player):
                possible_plants.append(PlantData("Squash-spreader", 800, 800, 400, ["Threepeater", "Squash"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Star Blover", 800,  0, 800, ["Fume-shroom", "Hypno-shroom"], []))
            if state.has("Starfruit", player) and state.has("Blover", player):
                possible_plants.append(PlantData("Star Blover", 750, 750, 375, ["Starfruit", "Blover"], []))
            if state.has("Fume-shroom", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Soot-shroom", 750, 0, 750, ["Fume-shroom", "Doom-shroom"], []))
            if state.has("Fume-shroom", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Foul-shroom", 700, 0, 700, ["Fume-shroom", "Garlic"], []))
            if state.has("Scaredy-shroom", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Frenzy-shroom", 600, 0, 300, ["Scaredy-shroom", "Doom-shroom"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Frost-shroom", 600, 0, 600, ["Fume-shroom", "Ice-shroom"], ["applies_cryo"]))
            if state.has("Threepeater", player) and state.has("Jalapeno", player):
                possible_plants.append(PlantData("Scorched Threepeater", 600, 200, 300, ["Threepeater","Jalapeno"], ["straight_shooter"]))
            if state.has("Melon-pult", player) and state.has("Cabbage-pult", player):
                possible_plants.append(PlantData("Cracked Melon", 600, 0, 600, ["Melon-pult", "Cabbage-pult"], []))
            if state.has("Cactus", player) and state.has("Plantern", player):
                possible_plants.append(PlantData("Lumos Cactus", 500, 0, 250, ["Cactus","Plantern"], ["targets_air"]))
            if state.has("Starfruit", player) and state.has("Magnet-shroom", player):
                possible_plants.append(PlantData("Starmorph", 500, 300, 250, ["Starfruit","Magnet-shroom"], []))
            if state.has("Fume-shroom", player) and state.has("Jalapeno", player):
                possible_plants.append(PlantData("Flame-shroom", 500, 0, 500, ["Fume-shroom","Jalapeno"], ["doesnt_freeze"]))
            if state.has("Melon-pult", player):
                possible_plants.append(PlantData("Melon-pult", 500, 0, 500, ["Melon-pult"], []))
            if state.has("Spruce Ballista", player) and (state.has("Spruce Sharpshooter", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Spruce Ballista", 500, 0, 250, ["Spruce Sharpshooter","Spruce Ballista"], ["no_pool","from_fertilizer","doesnt_freeze"]))
            if state.has("Cattail Girl", player) and "water" in modifier_flags:
                possible_plants.append(PlantData("Cattail Girl", 500, 500, 0, ["Cattail Girl"], []))
            if state.has("Threepeater", player) and state.has("Potato Mine", player):
                possible_plants.append(PlantData("Potato Spreader", 500, 150, 150, ["Threepeater","Potato Mine"], ["no_pool","straight_shooter"]))
            if state.has("Peashooter", player):
                possible_plants.append(PlantData("Gatling Pea", 400, 0, 200, ["Peashooter"], ["torchwood_usable","straight_shooter"]))
            if state.has("Scaredy-shroom", player) and state.has("Fume-shroom", player):
                possible_plants.append(PlantData("Gutsy-shroom", 350, 0, 175, ["Scaredy-shroom","Fume-shroom"], []))
            if state.has("Scaredy-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Shivery-shroom", 300, 0, 150, ["Scaredy-shroom","Ice-shroom"], ["applies_cryo","straight_shooter"]))
            if state.has("Threepeater", player):
                possible_plants.append(PlantData("Threepeater", 300, 100, 150, ["Threepeater"], ["torchwood_usable","straight_shooter"]))
            if state.has("Peashooter", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Doom Pea", 300, 0, 150, ["Peashooter","Doom-shroom"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Magnet-shroom", player):
                possible_plants.append(PlantData("Morph-shroom", 300, 0, 300, ["Fume-shroom"], []))
            if state.has("Cabbage-pult", player) and state.has("Kernel-pult", player):  # with sunflower
                possible_plants.append(PlantData("Taco-pult", 250, 0, 250, ["Cabbage-pult","Kernel-pult"], []))
            if state.has("Burger Blaster", player) and state.has("Cactus", player):  # with sunflower
                possible_plants.append(PlantData("Burger Blaster", 250, 0, 125, ["Burger Blaster","Cactus"], ["straight_shooter"]))
            if state.has("Lily Pad", player) and "water" in modifier_flags and (state.has("Cattail", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Cattail", 250, 250, 125, ["Lily Pad", "Cattail"],["from_fertilizer"]))
            if state.has("Starfruit", player):
                possible_plants.append(PlantData("Starfruit", 250, 150, 125, ["Starfruit"], []))
            if state.has("Fume-shroom", player) and (state.has("Gloom-shroom", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Gloom-shroom", 250, 250, 125, ["Fume-shroom","Gloom-shroom"], []))
            if state.has("Cabbage-pult", player) and state.has("Marigold", player):
                possible_plants.append(PlantData("Golden Cabbage", 200, 200, 200, ["Cabbage-pult","Marigold"], ["scuba_no_lilypad"]))
            if state.has("Magnet-shroom", player) and state.has("Cactus", player):
                possible_plants.append(PlantData("Magnethorn", 200, 0, 100, ["Cactus","Magnet-shroom"], ["targets_air","straight_shooter"]))
            if state.has("Spruce Sharpshooter", player):
                possible_plants.append(PlantData("Spruce Sharpshooter", 200, 0, 100, ["Spruce Sharpshooter"], []))
            if state.has("Puff-shroom", player):
                possible_plants.append(PlantData("Puff-shroom", 150, 0, 150, ["Puff-shroom"], ["straight_shooter"]))
            if state.has("Fume-shroom", player):
                possible_plants.append(PlantData("Fume-shroom", 150, 0, 150, ["Fume-shroom"], []))
            if state.has("Burger Blaster", player): #with sunflower
                possible_plants.append(PlantData("Burger Blaster", 150, 0, 75, ["Burger Blaster"], ["straight_shooter"]))
            if state.has("Scaredy-shroom", player) and state.has("Potato Mine", player):
                possible_plants.append(PlantData("Spuddy-shroom",150,0,75,["Scaredy-shroom","Potato Mine"],["straight_shooter"]))
            if state.has("Cactus", player):
                possible_plants.append(PlantData("Cactus",100,0,50,["Cactus"],["targets_air","straight_shooter"]))
            if state.has("Kernel-pult", player):
                possible_plants.append(PlantData("Kernel-pult", 100, 0, 100, ["Kernel-pult"], []))
            if state.has("Cabbage-pult", player):
                possible_plants.append(PlantData("Cabbage-pult", 100, 0, 100, ["Cabbage-pult"], []))
            if state.has("Scaredy-shroom", player):
                possible_plants.append(PlantData("Scaredy-shroom",100,0,40,["Scaredy-shroom"],["straight_shooter"]))
            if state.has("Aqua Aloe", player):
                possible_plants.append(PlantData("Aqua Aloe",50,0,50,["Aqua Aloe"],[]))
            break


        for i in range(state.count("Seed Slot", player)):
                free_slots += 1

        #first adjust all needed power levels from things such as torchwood

        #if you have lily pad, use normal power as pool power
        #otherwise search for the highest possible pool power
        #calculate pool power
        if "water" in modifier_flags:
            if state.has("Lily Pad", player):
                for i in possible_plants:
                    if "no_pool" in i.special_tags:
                        i.power = 0
            else:
                return False #TODO this
                #check for pool power
                #do this by both checking possible_plants and things like sea shroom
                #if one is greater than pool power then use that
                #if both are greater than pool power then pick the one with the least seed slots needed
                #if neither then check if both combined can do it, and if not immediately return false

                #if scuba flag and selected plant cant deal with it, pick again
                #last resort, try to fit a scuba counter alongside a stronger attacker

            if "scuba" in modifier_flags:
                if not state.has("Lily Pad", player):
                    return False #TODO this
                    #add highest counter possible to final_plants




        if "roof" in modifier_flags:

            if "sloped" in modifier_flags:

                if "3_starting_pots" in modifier_flags:
                    if not state.has("Flower Pot", player):
                        for i in possible_plants:
                            if "straight_shooter" in i.special_tags:
                                i.power = 0
                        level_strength += 200
                    else:
                        final_plants.append("Flower Pot")
                        for i in possible_plants:
                            i.power = i.roof_power

                elif "6_starting_pots" in modifier_flags:

                    for i in possible_plants:
                            i.power = i.roof_power

                    if state.has("Flower Pot", player):
                        final_plants.append("Flower Pot")
                    else:
                        level_strength += 200
                #less staring penelty

            else:
                if not state.has("Flower Pot",player):
                    return False#literally cant plant if no pots #passionfruit can be planted on roof
                else:
                    final_plants.append("Flower Pot")



        #for all special zombies, always use the minimum required seed slots so it doesnt have to be recalculated later
        #then prioritize whichever plants appear most often in other special solutions




        if "snowball_launcher" in modifier_flags:
            #not fume shroom
            pass #TODO this



        if "melon_pogo" in modifier_flags:
            if state.has("Chomper",player):
                final_plants.append("Chomper")
            elif state.has("Wall-nut",player) and state.has("Fertlizer",player):
                final_plants.append("Wall-nut")
            elif state.has("Burger Blaster",player):
                final_plants.append("Burger Blaster")
            elif state.has("Umbrella Leaf",player):
                final_plants.append("Umbrella Leaf")
            elif state.has("Wall-nut",player) and state.has("Tall-nut",player):
                final_plants.append("Wall-nut")
                final_plants.append("Tall-nut")
#technically any umbrella fusion except umbrella itself does this but ehhhh
            else:
                level_strength += 400

        #TODO torhcwood doubles all plants with the corresponding tag
        while True:
            needs_explosives = 0
            #pick best plant and combine into optimal seeds to bring
            selected_plant = None
            for i in possible_plants:
                i = possible_plants.pop(0)
                if "snow" in modifier_flags:
                    needs_explosives = 0
                    if not state.has("Firnace", player):
                        if "doesnt_freeze" in i.special_tags:
                            i.power = i.power - 400
                        else:
                            if "long_snow" in modifier_flags:
                                return False
                            else:
                                needs_explosives = 1
                    elif "Firnace" not in final_plants:
                        final_plants.append("Firnace")
                #massive penelty if you dont have firnace, lessen penalty if current best plant cant be frozen
                if "shieldbearer" in modifier_flags:
                    if "straight_shooter" in i.special_tags:
                        continue
                if "from_fertilizer" in i.special_tags and state.has("Fertilizer", player):
                    i.required_plants.pop()
                selected_plant = i
                break
            if selected_plant is None:
                return False
            if needs_explosives == 1:
                if state.has("Cherry Bomb", player):
                    if "Cherry Bomb" not in final_plants:
                        final_plants.append("Cherry Bomb")
                        level_strength += 600
                elif state.has("Doom-shroom", player):
                    if "Doom-shroom" not in final_plants:
                        final_plants.append("Doom-shroom")
                        level_strength += 600
                else:
                    return False#no way to unfreeze plants
            final_plants = list(set(final_plants))
            if len(final_plants) >= free_slots:
                return False
            for i in selected_plant.required_plants:
                if i not in final_plants:
                    final_plants.append(i)
            if len(final_plants) > free_slots:
                continue

            break
            # technically mowers can solve 1 flag levels

            # if the plant is a straightshooter, next plant
            #reasoning is that shield will reflect it back so its not worth it

        #if combined needed plants exceeds seed slots, pick again
        # if it reaches the end return false


        #add bonuses here for instas already in final_plants

        while len(final_plants) < free_slots:
            if state.has("Doom-shroom", player):
                if "Doom-shroom" not in final_plants:
                    final_plants.append("Doom-shroom")
                    level_strength -= 500
                    continue
            if state.has("Wall-nut", player):
                if "Wall-nut" not in final_plants:
                    final_plants.append("Wall-nut")
                    level_strength -= 300
                    continue
            if state.has("Cherry Bomb", player):
                if "Cherry Bomb" not in final_plants:
                    final_plants.append("Cherry Bomb")
                    level_strength -= 250
                    continue
            if state.has("Jalapeno", player):
                if "Jalapeno" not in final_plants:
                    final_plants.append("Jalapeno")
                    level_strength -= 200
                    continue
            if state.has("Neko Squash", player):
                if "Neko Squash" not in final_plants:
                    final_plants.append("Neko Squash")
                    level_strength -= 150
                    continue
            if state.has("Hypno-shroom", player):
                if "Hypno-shroom" not in final_plants:
                    final_plants.append("Hypno-shroom")
                    level_strength -= 150
                    continue
            if state.has("Squash", player):
                if "Squash" not in final_plants:
                    final_plants.append("Squash")
                    level_strength -= 100
                    continue
            # add explosive/ support bonuses
            # ie cherry bomb, umbrella rind, walls
            break



        #add mallet strength

        if selected_plant.power >= level_strength:
            return True
        else:
            return False
        # potato mine +50 (bonus if best fusion is expensive)
        #chomper - usefulness scales with zombie types/ available walls/ roof

        # tall-nut +400

        #puffshroom 250 (night) 100 (day)
        #fume-shroom 150
        #scaredy 100
        #hypno +100
        #ice-shroom +250
        #doom-shroom +2000  (2000 pool coverage)
        #gloomshroom +400 (increases w/ a wall/garlic)

        #squash +100
        #threepeater = 300 (100 pool coverage)
        #kelp +100 pool
        #jalapeno +300
        #torchwood (doubles best fusion's power if usable)
        #spikeweed +50 (bonus agaisnt vehicles)
        # spikerock +200

        #seashroom +100 pool
        #plantern +0
        #blover +0 (balloon bonus)
        #cactus 100 (balloon bonus)
        #starfruit 250 (200 pool coverage)
        #pumpkin +200
        #magnetshroom (bonus against metal)

        #umbrella (bonus against bungee, catapult etc)
        # garlic +50
        # marigold +0
        # melonpult +500 (lobber)
        #cob cannon +1500 (1500 pool coverage)

        #firnace  (disables snow flag)
        #spruce = 200 (200 snow power)
        #aqua aloe = dude idk
        #snow lotus (power only with snow zombies)
        #bamblock +100





        #swordmastar = 800 (800 pool coverage)
        #cattail girl =400 (400 pool coverage)
        #burger-blaster = 200 (bonus if no sunflower) (bonus if required upgrade plants & seed slots are available)\
        #sniper pea = 0 (temp)
        #queen endoflame = 0 (temp)
        #double passion = +200
        # ice lily = 200 (doubles best fusion if it doesnt have iceshroom or inflamed)
        # pearmafrost +250
        # crysanctum +50?
        #neko squash +250
        #ampnion = 500
        #coldsnap bean (doubles snow pea fusions without cold torchwood)

        #shovel +0
        #gloves +500 (only if current power is high)
        #mallet +300















        #day9 = state.has("Cherry Bomb", player)
        #night18 = state.has("Night Access", player)
        #pool27 = state.has("Pool Access", player) and state.has("Lily Pad", player) and state.has("Threepeater", player) and state.has("Squash", player) and state.has("Spikeweed", player) and state.has("Torchwood", player) and state.has("Jalapeno", player)
        #fog36 = state.has("Fog Access", player) and state.has("Cactus", player) and state.has("Starfruit", player) and state.has("Plantern", player) and state.has("Magnet-shroom", player) and state.has("Pumpkin", player)
        #roof45 = state.has("Roof Access", player) and state.has("Flower Pot", player) and state.has("Melon-pult", player) and state.has("Kernel-pult", player) and state.has("Cabbage-pult", player) and state.has("Umbrella Leaf", player) and state.has("Ice-shroom", player)
        #snow9 = state.has("Snow Access", player) and state.has("Firnace", player) and state.has("Spruce Sharpshooter", player) and state.has("Saw-me-not", player) and state.has("Aloe Aqua", player) and state.has("Snow Lotus", player) and state.has("Bamblock", player)
        #return day9 and night18 and pool27 and fog36 and roof45 and snow9

    rf = RuleFactory(world, options, player, move_rando_bitvec)

    connect_regions(world, player, "Menu", "Day", lambda state: state.has("Day Access", player))
    connect_regions(world, player, "Menu", "Night", lambda state: state.has("Night Access", player))
    connect_regions(world, player, "Menu", "Pool", lambda state: state.has("Pool Access", player))
    connect_regions(world, player, "Menu", "Fog", lambda state: state.has("Fog Access", player))
    connect_regions(world, player, "Menu", "Roof", lambda state: state.has("Roof Access", player))
    connect_regions(world, player, "Menu", "Snow", lambda state: state.has("Snow Access", player))
    if options.challenge_sanity:
        connect_regions(world, player, "Menu", "Fusion Challenges", lambda state: state.has("Fusion Challenge Access", player))
    if options.showcase_sanity:
        connect_regions(world, player, "Menu", "Fusion Showcase", lambda state: state.has("Fusion Showcase Access", player))






    cherry_newspaper = "CHER & WALL | CHER & PUMPK | CHER & MEL | CHER & PEA & GLOV"
    base_pool = "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT"
    base_snow = "FIR | CHER | DOOM | ICE | JAL"
    balloon_pool = "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT" #the way i understand how rf makes rules means i can't combine different strings
    # parenthesis aren't supported by rf so
    #though adding different rules to the same l ocation might work depending on how thats handled

    add_rule(world.get_location("Day: Level 3 (1)", player),
             lambda state: can_beat_power_level(state, 200,[]))
    add_rule(world.get_location("Day: Level 3 (2)", player),
             lambda state: can_beat_power_level(state, 200,[]))

    add_rule(world.get_location("Day: Level 5 (1)", player),
             lambda state: can_beat_power_level(state, 200,[]))
    add_rule(world.get_location("Day: Level 5 (2)", player),
             lambda state: can_beat_power_level(state, 200,[]))

    add_rule(world.get_location("Day: Level 6 (1)", player),
             lambda state: can_beat_power_level(state, 800,[]))#chomperma
    add_rule(world.get_location("Day: Level 6 (2)", player),
             lambda state: can_beat_power_level(state, 800,[]))

    add_rule(world.get_location("Day: Level 7 (1)", player),
             lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"]))  # chomperma
    add_rule(world.get_location("Day: Level 7 (2)", player),
             lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"]))

    add_rule(world.get_location("Day: Level 8 (1)", player),
             lambda state: can_beat_power_level(state, 400,[]))
    add_rule(world.get_location("Day: Level 8 (2)", player),
             lambda state: can_beat_power_level(state, 400,[]))

    add_rule(world.get_location("Day: Level 9 (1)", player),
             lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"]))  # chomperma and cherry nut
    add_rule(world.get_location("Day: Level 9 (2)", player),
             lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"]))

    add_rule(world.get_location("Night: Level 11 (1)", player),
             lambda state: can_beat_power_level(state, 200, []))  # screen door
    add_rule(world.get_location("Night: Level 11 (2)", player),
             lambda state: can_beat_power_level(state, 200, []))

    add_rule(world.get_location("Night: Level 12 (1)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 12 (2)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 13 (1)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 13 (2)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 14 (1)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 14 (2)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 15 (1)", player),
             lambda state: can_beat_power_level(state, 350, [])) #dancing vaulter
    add_rule(world.get_location("Night: Level 15 (2)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 16 (1)", player),
             lambda state: can_beat_power_level(state, 350, [])) #dancing vaulter +
    add_rule(world.get_location("Night: Level 16 (2)", player),
             lambda state: can_beat_power_level(state, 350, []))
    add_rule(world.get_location("Night: Level 17 (1)", player),
             lambda state: can_beat_power_level(state, 600, [])) #dancing vaulter +
    add_rule(world.get_location("Night: Level 17 (2)", player),
             lambda state: can_beat_power_level(state, 600, []))
    add_rule(world.get_location("Night: Level 18 (1)", player),
             lambda state: can_beat_power_level(state, 800, [])) #dancing vaulter +
    add_rule(world.get_location("Night: Level 18 (2)", player),
             lambda state: can_beat_power_level(state, 800, []))

    add_rule(world.get_location("Pool: Level 19 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Pool: Level 19 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Pool: Level 20 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["water"]))
    add_rule(world.get_location("Pool: Level 20 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["water"]))
    add_rule(world.get_location("Pool: Level 21 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","scuba"]))
    add_rule(world.get_location("Pool: Level 21 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","scuba"]))
    add_rule(world.get_location("Pool: Level 22 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water"]))
    add_rule(world.get_location("Pool: Level 22 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water"]))
    add_rule(world.get_location("Pool: Level 23 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water"]))
    add_rule(world.get_location("Pool: Level 23 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water"]))
    add_rule(world.get_location("Pool: Level 24 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))
    add_rule(world.get_location("Pool: Level 24 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))
    add_rule(world.get_location("Pool: Level 25 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))
    add_rule(world.get_location("Pool: Level 25 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))
    add_rule(world.get_location("Pool: Level 26 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))
    add_rule(world.get_location("Pool: Level 26 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"]))

    add_rule(world.get_location("Fog: Level 28 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Fog: Level 28 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Fog: Level 29 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Fog: Level 29 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water"]))
    add_rule(world.get_location("Fog: Level 30 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon"]))
    add_rule(world.get_location("Fog: Level 30 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon"]))
    add_rule(world.get_location("Fog: Level 31 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["water","balloon"]))#screen door
    add_rule(world.get_location("Fog: Level 31 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["water","balloon"]))
    add_rule(world.get_location("Fog: Level 32 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon"]))#screen door
    add_rule(world.get_location("Fog: Level 32 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon"]))
    add_rule(world.get_location("Fog: Level 33 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger"]))
    add_rule(world.get_location("Fog: Level 33 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger"]))
    add_rule(world.get_location("Fog: Level 34 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger"]))
    add_rule(world.get_location("Fog: Level 34 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger"]))
    add_rule(world.get_location("Fog: Level 35 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["water","balloon"]))#clown in the box
    add_rule(world.get_location("Fog: Level 35 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["water","balloon"]))

    add_rule(world.get_location("Roof: Level 37 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["roof","sloped","6_starting_pots"]))
    add_rule(world.get_location("Roof: Level 37 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["roof","sloped","6_starting_pots"]))
    add_rule(world.get_location("Roof: Level 38 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 38 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 39 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 39 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 40 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 40 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 41 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","mecha_nut"])) #bungee + catapult
    add_rule(world.get_location("Roof: Level 41 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","mecha_nut"]))
    add_rule(world.get_location("Roof: Level 42 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","melon_pogo"])) #catapult
    add_rule(world.get_location("Roof: Level 42 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","melon_pogo"]))

    add_rule(world.get_location("Roof: Level 43 (1)", player),
             lambda state: can_beat_power_level(state, 500, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 43 (2)", player),
             lambda state: can_beat_power_level(state, 500, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Roof: Level 44 (1)", player),
             lambda state: can_beat_power_level(state, 600, ["roof","sloped","3_starting_pots"]))#cherry catapult
    add_rule(world.get_location("Roof: Level 44 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["roof","sloped","3_starting_pots"]))
    add_rule(world.get_location("Snow: Level 1 (1)", player),
             lambda state: can_beat_power_level(state, 150, ["snow"]))
    add_rule(world.get_location("Snow: Level 1 (2)", player),
             lambda state: can_beat_power_level(state, 150, ["snow"]))
    add_rule(world.get_location("Snow: Level 2 (1)", player),
             lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer"]))
    add_rule(world.get_location("Snow: Level 2 (2)", player),
             lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer"]))
    add_rule(world.get_location("Snow: Level 3 (1)", player),
             lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer","long_snow"]))
    add_rule(world.get_location("Snow: Level 3 (2)", player),
             lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer","long_snow"]))
    add_rule(world.get_location("Snow: Level 4 (1)", player),
             lambda state: can_beat_power_level(state, 250, ["snow","shieldbearer","long_snow","snowball_launcher"]))
    add_rule(world.get_location("Snow: Level 4 (2)", player),
             lambda state: can_beat_power_level(state, 250, ["snow","shieldbearer","long_snow","snowball_launcher"]))
    add_rule(world.get_location("Snow: Level 5 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["snow","shieldbearer","long_snow","snowball_launcher"]))
    add_rule(world.get_location("Snow: Level 5 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["snow","shieldbearer","long_snow","snowball_launcher"]))
    add_rule(world.get_location("Snow: Level 6 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher", "furling"]))#yeti
    add_rule(world.get_location("Snow: Level 6 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher", "furling"]))
    add_rule(world.get_location("Snow: Level 7 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher"]))#yeti
    add_rule(world.get_location("Snow: Level 7 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher"]))



    add_rule(world.get_location("Snow: Level 8 (1)", player),
             lambda state: can_beat_power_level(state, 600,["snow", "shieldbearer", "long_snow", "furling"]))  # yeti
    add_rule(world.get_location("Snow: Level 8 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["snow", "shieldbearer", "long_snow", "furling"]))


#cattail girl carries without sunflowers btw
    rf.assign_rule("Pool: Level 27 (1)", "LILY+THRE+SQUA+SPIK+TORCH+WALL+JAL")#kelp also here but its not necessary
    rf.assign_rule("Pool: Level 27 (2)", "LILY+THRE+SQUA+SPIK+TORCH+WALL+JAL")

    rf.assign_rule("Fog: Level 36 (1)", "LILY+CACT+STAR+PLANT+MAGN+PUMPK")  # maybe allow bombs?
    rf.assign_rule("Fog: Level 36 (2)", "LILY+CACT+STAR+PLANT+MAGN+PUMPK")

    rf.assign_rule("Roof: Level 45 (1)", "POT+MEL+KER+CAB+UMBRE+ICE+JAL")
    rf.assign_rule("Roof: Level 45 (2)", "POT+MEL+KER+CAB+UMBRE+ICE+JAL")

    rf.assign_rule("Snow: Level 9 (1)", "FIR+SPRUC+SAW+ALOE+LOTUS+BAMB")
    rf.assign_rule("Snow: Level 9 (2)", "FIR+SPRUC+SAW+ALOE+LOTUS+BAMB")



#fume shroom and melon pult have jalapeno fusions
#peashooter fume scaredy melon have ice shroom fusions
#spruce supershooter?



#
##snow levels require explosives or firnace btw
#

    if options.challenge_sanity:
        add_rule(world.get_location("Fusion Challenge: Explod-o-shooter (1)", player),
                 lambda state: can_beat_power_level(state, 300,["cherry_newspaper"]))
        add_rule(world.get_location("Fusion Challenge: Explod-o-shooter (2)", player),
                 lambda state: can_beat_power_level(state, 300, ["cherry_newspaper"]))
        add_rule(world.get_location("Fusion Challenge: Chompzilla (1)", player),
                 lambda state: can_beat_power_level(state, 800,[]))
        add_rule(world.get_location("Fusion Challenge: Chompzilla (2)", player),
                 lambda state: can_beat_power_level(state, 800, []))
        add_rule(world.get_location("Fusion Challenge: Charm-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 600,[]))
        add_rule(world.get_location("Fusion Challenge: Charm-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 600, []))
        add_rule(world.get_location("Fusion Challenge: Doomspike-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 600,[]))
        add_rule(world.get_location("Fusion Challenge: Doomspike-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 600, []))
        add_rule(world.get_location("Fusion Challenge: Infernowood (1)", player),
                 lambda state: can_beat_power_level(state, 600,[]))
        add_rule(world.get_location("Fusion Challenge: Infernowood (2)", player),
                 lambda state: can_beat_power_level(state, 600, []))
        add_rule(world.get_location("Fusion Challenge: Krakerberus (1)", player),
                 lambda state: can_beat_power_level(state, 200, ["water", "scuba"]))
        add_rule(world.get_location("Fusion Challenge: Krakerberus (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["water", "scuba"]))
        add_rule(world.get_location("Fusion Challenge: Stardrop (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","cherry_newspaper","balloon"]))
        add_rule(world.get_location("Fusion Challenge: Stardrop (2)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","cherry_newspaper","balloon"]))
        add_rule(world.get_location("Fusion Challenge: Bloverthorn Pumpkin (1)", player),
                 lambda state: can_beat_power_level(state, 300, ["water"]))
        add_rule(world.get_location("Fusion Challenge: Bloverthorn Pumpkin (2)", player),
                 lambda state: can_beat_power_level(state, 300, ["water"]))
        add_rule(world.get_location("Fusion Challenge: Salad-pult (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped"]))
        add_rule(world.get_location("Fusion Challenge: Salad-pult (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped"]))
        add_rule(world.get_location("Fusion Challenge: Alchemist Umbrella (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped","mecha_nut"]))
        add_rule(world.get_location("Fusion Challenge: Alchemist Umbrella (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped","mecha_nut"]))
        add_rule(world.get_location("Fusion Challenge: Spruce Supershooter (1)", player),
                 lambda state: can_beat_power_level(state, 600, ["snow","cherry_newspaper","shieldbearer"]))
        add_rule(world.get_location("Fusion Challenge: Spruce Supershooter (2)", player),
                 lambda state: can_beat_power_level(state, 600, ["snow","cherry_newspaper","shieldbearer"]))

    if options.showcase_sanity:
        add_rule(world.get_location("Fusion Showcase: Titan Pea Turret (1)", player),
                 lambda state: can_beat_power_level(state, 800,["cherry_newspaper"]))
        add_rule(world.get_location("Fusion Showcase: Titan Pea Turret (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"]))
        add_rule(world.get_location("Fusion Showcase: Pumpkin Bunker (1)", player),
                 lambda state: can_beat_power_level(state, 800,["water","cherry_newspaper","balloon"]))#more seed slots needed
        add_rule(world.get_location("Fusion Showcase: Pumpkin Bunker (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["water","cherry_newspaper","balloon"]))
        add_rule(world.get_location("Fusion Showcase: Nugget-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 800,[]))
        add_rule(world.get_location("Fusion Showcase: Nugget-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 800, []))
        add_rule(world.get_location("Fusion Showcase: Spuddy-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 300,[]))
        add_rule(world.get_location("Fusion Showcase: Spuddy-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 300, []))
        add_rule(world.get_location("Fusion Showcase: Foul-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 800,[]))
        add_rule(world.get_location("Fusion Showcase: Foul-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 800, []))
        add_rule(world.get_location("Fusion Showcase: Boomwood (1)", player),
                 lambda state: can_beat_power_level(state, 800,[]))
        add_rule(world.get_location("Fusion Showcase: Boomwood (2)", player),
                 lambda state: can_beat_power_level(state, 800, []))
        add_rule(world.get_location("Fusion Showcase: Spike-nut (1)", player),
                 lambda state: can_beat_power_level(state, 200,["water"]))
        add_rule(world.get_location("Fusion Showcase: Spike-nut (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["water"]))
        add_rule(world.get_location("Fusion Showcase: Leviathan-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 200,["water","scuba"]))
        add_rule(world.get_location("Fusion Showcase: Leviathan-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["water","scuba"]))

        rf.assign_rule("Fusion Showcase: Explod-o-tato Mine (1)","PEA+WALL+JAL+CHER+MINE")
        rf.assign_rule("Fusion Showcase: Explod-o-tato Mine (2)","PEA+WALL+JAL+CHER+MINE")

        rf.assign_rule("Fusion Showcase: Chomper Maw (1)", "JIQ+CHOM")#possible with only fused chompers
        rf.assign_rule("Fusion Showcase: Chomper Maw (2)", "JIQ+CHOM")#possible with only fused chompers

        rf.assign_rule("Fusion Showcase: Mind-blover (1)", "HYP+MAGN+BLOV")
        rf.assign_rule("Fusion Showcase: Mind-blover (2)", "HYP+MAGN+BLOV")

        rf.assign_rule("Fusion Showcase: Bamboom (1)", "CHER+JAL")
        rf.assign_rule("Fusion Showcase: Bamboom (2)", "CHER+JAL")


    #scaredy's dream requires some fuckass strategies glove+pea | a bunch of instants |

    if options.goal_type == 0:
        rf.assign_rule("Dr. Zomboss' Revenge", "POT+KER+CAB+MEL+UMBRE+MARI+GLOV+ICE+JAL+MAL")
        world.completion_condition[player] = lambda state: state.has("Roof Access", player) and state.has("Flower Pot",player) and state.has("Kernel-pult", player) and state.has("Cabbage-pult", player) and state.has("Melon-pult",player) and state.has("Marigold",player) and state.has("Plant Gloves", player) and state.has("Mallet", player) and state.has("Jalapeno", player) and state.has("Ice-shroom", player)

    if options.goal_type == 1:
        world.completion_condition[player] = lambda state: state.can_reach("Day: Level 9 (1)", "Location", player) and state.can_reach("Night: Level 18 (1)", "Location", player) and state.can_reach("Pool: Level 27 (1)", "Location", player) and state.can_reach("Fog: Level 36 (1)", "Location", player) and state.can_reach("Roof: Level 45 (1)", "Location", player) and state.can_reach("Snow: Level 9 (1)", "Location", player)
        #chompzilla requires high dps
        #most challenges do
    # rf.assign_rule("Fusion Challenge: Stardrop","LILY+(CACT|CAT|FERT)")  this can almost clear itself with glove+mallet lmao
#    #if options.completion_type == 0:
        #world.completion_condition[player] = lambda state: (state.has("Snow Access", player) and state.has("Firnace", player)
    #    and state.has("Spruce Sharpshooter", player) and state.has("Bamblock", player) and state.has(
    #    "Saw-me-not", player) and state.has("Aloe Aqua", player))
    #else:
    #    world.completion_condition[player] = lambda state: state.can_reach("Credits", 'Region', player)



class RuleFactory:

    world: MultiWorld
    player: int
    move_rando_bitvec: bool
    area_randomizer: bool
    capless: bool
    cannonless: bool
    moveless: bool

    token_table = {
        "PEA": "Peashooter",
        "SUN": "Sunflower",
        "CHER": "Cherry Bomb",
        "WALL": "Wall-nut",
        "MINE": "Potato Mine",
        "CHOM": "Chomper",
        "PUFF": "Puff-shroom",
        "FUM": "Fume-shroom",
        "HYP": "Hypno-shroom",
        "ICE": "Ice-shroom",
        "DOOM": "Doom-shroom",
        "GLOOM": "Gloom-shroom",
        "LILY": "Lily Pad",
        "SQUA": "Squash",
        "THRE": "Threepeater",
        "JAL": "Jalapeno",
        "SPIK":"Spikeweed",
        "TORCH": "Torchwood",
        "SEA":"Sea-shroom",
        "PLANT": "Plantern",
        "CACT":"Cactus",
        "BLOV": "Blover",
        "STAR": "Starfruit",
        "PUMPK": "Pumpkin",
        "MAGN": "Magnet-shroom",
        "CAT": "Cattail",
        "CAB": "Cabbage-pult",
        "POT": "Flower Pot",
        "KER": "Kernel-pult",
        "UMBRE": "Umbrella Leaf",
        "MARI": "Marigold",
        "MEL": "Melon-pult",
        "COB": "Cob Cannon",
        "JIQ": "Jicamagic",
        "FIR": "Firnace",
        "SPRUC": "Spruce Sharpshooter",
        "SAW": "Saw-me-not",
        "LOTUS": "Snow Lotus",
        "ALOE": "Aloe Aqua",
        "BAMB": "Bamblock",
        "CATGI": "Cattail Girl",
        "GLOV": "PLant Gloves",
        "MAL": "Mallet",
        "FERT": "Fertilizer",
    }

    class PVZFLogicException(Exception):
        pass

    def __init__(self, world, options: PVZFOptions, player: int, move_rando_bitvec: int):
        self.world = world
        self.player = player
        #self.move_rando_bitvec = move_rando_bitvec
        #self.area_randomizer = options.area_rando > 0
        #self.capless = not options.strict_cap_requirements
        #self.cannonless = not options.strict_cannon_requirements
        #self.moveless = not options.strict_move_requirements

    def assign_rule(self, target_name: str, rule_expr: str):
        target = self.world.get_location(target_name, self.player) if target_name in location_table else self.world.get_entrance(target_name, self.player)
        cannon_name = "Cannon Unlock " + target_name.split(':')[0]
        try:
            rule = self.build_rule(rule_expr, cannon_name)
        except RuleFactory.PVZFLogicException as exception:
            raise RuleFactory.PVZFLogicException(
                f"Error generating rule for {target_name} using rule expression {rule_expr}: {exception}")
        if rule:
            set_rule(target, rule)

    def build_rule(self, rule_expr: str, cannon_name: str = '') -> Callable:
        expressions = rule_expr.split(" | ")
        rules = []
        for expression in expressions:
            or_clause = self.combine_and_clauses(expression, cannon_name)
            if or_clause is True:
                return None
            if or_clause is not False:
                rules.append(or_clause)
        if rules:
            if len(rules) == 1:
                return rules[0]
            else:
                return lambda state: any(rule(state) for rule in rules)
        else:
            return None

    def combine_and_clauses(self, rule_expr: str, cannon_name: str) -> Union[Callable, bool]:
        expressions = rule_expr.split(" & ")
        rules = []
        for expression in expressions:
            and_clause = self.make_lambda(expression, cannon_name)
            if and_clause is False:
                return False
            if and_clause is not True:
                rules.append(and_clause)
        if rules:
            if len(rules) == 1:
                return rules[0]
            return lambda state: all(rule(state) for rule in rules)
        else:
            return True

    def make_lambda(self, expression: str, cannon_name: str) -> Union[Callable, bool]:
        if '+' in expression:
            tokens = expression.split('+')
            items = set()
            for token in tokens:
                item = self.parse_token(token, cannon_name)
                if item is True:
                    continue
                if item is False:
                    return False
                items.add(item)
            if items:
                return lambda state: state.has_all(items, self.player)
            else:
                return True
        if '/' in expression:
            tokens = expression.split('/')
            items = set()
            for token in tokens:
                item = self.parse_token(token, cannon_name)
                if item is True:
                    return True
                if item is False:
                    continue
                items.add(item)
            if items:
                return lambda state: state.has_any(items, self.player)
            else:
                return False
        if '{{' in expression:
            return lambda state: state.can_reach(expression[2:-2], "Location", self.player)
        if '{' in expression:
            return lambda state: state.can_reach(expression[1:-1], "Region", self.player)
        item = self.parse_token(expression, cannon_name)
        if item in (True, False):
            return item
        return lambda state: state.has(item, self.player)

    def parse_token(self, token: str, cannon_name: str) -> Union[str, bool]:
        item = self.token_table.get(token, None)
        if not item:
            raise Exception(f"Invalid token: '{item}'")

        return item

