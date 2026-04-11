from operator import truediv
from typing import Callable, Union, Dict, Set

from BaseClasses import MultiWorld, CollectionState
from ..generic.Rules import add_rule, set_rule
from .Locations import location_table
from .Options import PVZFOptions
from .Regions import connect_regions,SRB2Zones
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

    def can_beat_conveyor(state: CollectionState, required_plants, possible_plants, percentage):
        for plant in required_plants:
            if not state.has(plant,player):
                return False
        has_plants = 0.0
        max_plants = 0.0
        for plant in possible_plants:
            if state.has(plant,player):
                has_plants += 1.0
            max_plants += 1.0
        if max_plants == 0:
            return True
        if (has_plants/max_plants) >= percentage:
            return True
        return False

    def can_beat_power_level(state: CollectionState, level_strength,  modifier_flags, slots_used):
        #Breaks when it finds the highest possible plant sorted by power
        free_slots= 4-slots_used#-1 for sunflower
        possible_plants = []
        final_plants = []
        #check for required plants for things like balloon, scuba, cherry newspaper, sunflower, lily pad, pot to take away free_slots
        # if hard requirements arent found, immediately return false (balloon/scuba)
        if options.logic_difficulty == 0:
            if "snow" in modifier_flags:
                if not state.has("Firnace",player):
                    return False
            if "water" in modifier_flags:
                if not state.has("Lily Pad",player):
                    return False
            if "roof" in modifier_flags:
                if not state.has("Flower Pot",player):
                    return False
            if "night" in modifier_flags:
                if state.has("Sea-shroom",player) and "water" in modifier_flags:
                    free_slots -= 1
                    final_plants.append("Sea-shroom")
                elif state.has("Puff-shroom",player):
                    free_slots -= 1
                    final_plants.append("Puff-shroom")
                elif state.has("Scaredy-shroom",player):
                    free_slots -= 1
                    final_plants.append("Scaredy-shroom")
                elif state.has("Fume-shroom",player):
                    free_slots -= 1
                    final_plants.append("Fume-shroom")
                else:
                    return False

        if "compact_planting" in modifier_flags:
            if state.has("Puff-shroom",player):
                final_plants.append("Puff-shroom")
            else:
                return False


        if "cherry_newspaper" in modifier_flags:#todo review other cherry newspaper counters
            if state.has("Cherry Bomb",player) and state.has("Wall-nut",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Wall-nut")
            elif state.has("Cherry Bomb",player) and state.has("Pumpkin",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Pumpkin")
            elif state.has("Melon-pult",player) and state.has("Umbrella Leaf",player):
                final_plants.append("Melon-pult")
                final_plants.append("Umbrella Leaf")
            elif state.has("Cherry Bomb",player) and state.has("Peashooter",player):
                final_plants.append("Peashooter")
                final_plants.append("Cherry Bomb")
            else:
                return False

        if "hard_cherry" in modifier_flags:
            if state.has("Cherry Bomb",player) and state.has("Wall-nut",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Wall-nut")
            elif state.has("Cherry Bomb",player) and state.has("Pumpkin",player):
                final_plants.append("Cherry Bomb")
                final_plants.append("Pumpkin")
            elif state.has("Cherry Bomb",player) and state.has("Peashooter",player):
                final_plants.append("Peashooter")
                final_plants.append("Cherry Bomb")
            else:
                return False




        if "catapult" in modifier_flags:
            pass
        #TODO require walls or umbrella leaf
        #if not roof then allow spikeweed
        if "snowball_launcher" in modifier_flags:
            if not state.has("Firnace",player):
                return False
            else:
                final_plants.append("Firnace")

        if "flag_capture" in modifier_flags:
            if state.has("Hypno-shroom", player):
                final_plants.append("Hypno-shroom")
            else:
                return False#todo needs more



        if "balloon" in modifier_flags: #cob cannon CAN target balloons
            if "tough_balloon" in modifier_flags:
                if state.has("Snipea",player) and "odyssey" in modifier_flags:
                    final_plants.append("Snipea")
                elif state.has("Lily Pad", player) and state.has("Fertilizer", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                elif state.has("Cattail Girl",player) and "water" in modifier_flags:
                    final_plants.append("Cattail Girl")
                elif state.has("Cactus",player) and ("water" not in modifier_flags or state.has("Lily Pad", player)):
                    final_plants.append("Cactus")
                elif state.has("Cactus",player) and state.has("Sea-shroom",player) and "water" in modifier_flags:
                    final_plants.append("Cactus")
                    final_plants.append("Sea-shroom")
                elif state.has("Cactus",player) and state.has("Starfruit",player):
                    final_plants.append("Cactus")
                    final_plants.append("Starfruit")
                elif state.has("Cactus",player) and state.has("Blover",player) and state.has("Plant Gloves",player):
                    final_plants.append("Cactus")
                    final_plants.append("Blover")
                elif state.has("Lily Pad", player) and state.has("Cattail", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                    final_plants.append("Cattail")
                else:
                    return False

            else:
                if state.has("Snipea",player) and "odyssey" in modifier_flags:
                    final_plants.append("Snipea")
                elif state.has("Lily Pad", player) and state.has("Fertilizer", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                elif state.has("Blover",player):
                    final_plants.append("Blover")
                elif state.has("Cattail Girl",player) and "water" in modifier_flags:
                    final_plants.append("Cattail Girl")
                elif state.has("Cactus",player) and ("water" not in modifier_flags or state.has("Lily Pad", player)):
                    final_plants.append("Cactus")
                elif state.has("Cactus",player) and state.has("Sea-shroom",player) and "water" in modifier_flags:
                    final_plants.append("Cactus")
                    final_plants.append("Sea-shroom")
                elif state.has("Cactus",player) and state.has("Starfruit",player) and "water" in modifier_flags:
                    final_plants.append("Cactus")
                    final_plants.append("Starfruit")
                elif state.has("Cactus",player) and state.has("Blover",player) and state.has("Plant Gloves",player) and "water" in modifier_flags:
                    final_plants.append("Cactus")
                    final_plants.append("Blover")
                elif state.has("Lily Pad", player) and state.has("Cattail", player) and "water" in modifier_flags:
                    final_plants.append("Lily Pad")
                    final_plants.append("Cattail")
                else:
                    return False

        if "harder_balloon" in modifier_flags:
            if state.has("Snipea", player) and "odyssey" in modifier_flags:
                final_plants.append("Snipea")
            elif state.has("Cactus",player) and state.has("Blover",player) and state.has("Pumpkin",player):
                final_plants.append("Cactus")
                final_plants.append("Blover")
                final_plants.append("Pumpkin")
            elif state.has("Puff-shroom",player) and state.has("Peashooter",player) and state.has("Magnet-shroom",player) and state.has("Plant Gloves",player) and "odyssey" in modifier_flags:
                final_plants.append("Puff-shroom")
                final_plants.append("Peashooter")
                final_plants.append("Magnet-shroom")
            elif state.has("Cattail",player) and state.has("Nyan Squash",player) and "odyssey" in modifier_flags:
                final_plants.append("Cattail")
                final_plants.append("Nyan Squash")
            else:
                return False

            #bloverthorn pumpkin+laser pumpkin
            #aristocattail
            #buckshroom squad


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
            elif state.has("Snipea", player) and "odyssey" in modifier_flags:
                final_plants.append("Snipea")
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
        #aristocattail
        while True:
            if "high_gravity" in modifier_flags:
                if state.has("Amp-nion", player):
                    possible_plants.append(PlantData("Ampnion", 1200, 1200, 1200, ["Amp-nion"], []))
                if state.has("Melon-pult", player) and state.has("Kernel-pult", player) and state.has("Jicamagic",player):
                    possible_plants.append(PlantData("Melon Mortar", 1200, 1200, 1200, ["Melon-pult", "Kernel-pult", "Jicamagic"],["scuba_no_lilypad", "doesnt_freeze"]))
                if state.has("Kernel-pult", player) and (state.has("Cob Cannon", player) or state.has("Fertilizer", player)):
                    possible_plants.append(PlantData("Cob Cannon", 1000, 1000, 1000, ["Kernel-pult", "Cob Cannon"],["from_fertilizer", "scuba_no_lilypad", "doesnt_freeze"]))
                if state.has("Wall-nut", player) and state.has("Hypno-shroom", player):
                    possible_plants.append(PlantData("Hypno Nut", 1000, 0, 1000, ["Wall-nut", "Hypno-shroom"], []))
                if state.has("Fume-shroom", player) and state.has("Hypno-shroom", player):
                    possible_plants.append(PlantData("Perfume-shroom", 800, 0, 800, ["Fume-shroom", "Hypno-shroom"], []))
                if state.has("Fume-shroom", player) and state.has("Doom-shroom", player):
                    possible_plants.append(PlantData("Soot-shroom", 750, 0, 750, ["Fume-shroom", "Doom-shroom"], []))
                if state.has("Fume-shroom", player) and state.has("Garlic", player):
                    possible_plants.append(PlantData("Foul-shroom", 700, 0, 700, ["Fume-shroom", "Garlic"], []))
                if state.has("Fume-shroom", player) and state.has("Ice-shroom", player):
                    possible_plants.append(PlantData("Frost-shroom", 600, 0, 600, ["Fume-shroom", "Ice-shroom"], ["applies_cryo"]))
                if state.has("Fume-shroom", player) and state.has("Jalapeno", player):
                    possible_plants.append(PlantData("Flame-shroom", 500, 0, 500, ["Fume-shroom", "Jalapeno"], ["doesnt_freeze"]))
                if state.has("Saw-me-not", player):
                    possible_plants.append(PlantData("Saw-me-not", 400, 0, 400, ["Saw-me-not"], []))
                if state.has("Fume-shroom", player) and state.has("Magnet-shroom", player):
                    possible_plants.append(PlantData("Morph-shroom", 300, 0, 300, ["Fume-shroom", "Magnet-shroom"], []))
                if state.has("Fume-shroom", player) and (state.has("Gloom-shroom", player) or state.has("Fertilizer", player)):
                    possible_plants.append(PlantData("Gloom-shroom", 250, 250, 125, ["Fume-shroom", "Gloom-shroom"], []))
                if state.has("Fume-shroom", player):
                    possible_plants.append(PlantData("Fume-shroom", 150, 0, 150, ["Fume-shroom"], []))
                break


            if "compact_planting" in modifier_flags:
                if state.has("Peashooter", player) and state.has("Puff-shroom", player) and state.has("Ice-shroom", player):
                    possible_plants.append(PlantData("Icicle-shroom", 1200, 0, 400, ["Peashooter", "Puff-shroom","Ice-shroom"],["straight_shooter","applies_cryo"]))
                if state.has("Peashooter", player) and state.has("Puff-shroom", player):
                    possible_plants.append(PlantData("Pea-shroom", 900, 0, 300, ["Peashooter", "Puff-shroom"],["straight_shooter"]))
                if state.has("Puff-shroom", player) and state.has("Melon-pult", player):
                    possible_plants.append(PlantData("Slice-pult", 600, 0, 600, ["Melon-pult", "Puff-shroom"], []))
                if state.has("Puff-shroom", player) and state.has("Starfruit", player):
                    possible_plants.append(PlantData("Twinkle-shroom", 500, 0, 500, ["Starfruit", "Puff-shroom"], []))
                if state.has("Puff-shroom", player) and state.has("Kernel-pult", player):
                    possible_plants.append(PlantData("Popcorn-pult", 500, 0, 500, ["Kernel-pult", "Puff-shroom"], []))
                if state.has("Puff-shroom", player) and state.has("Cabbage-pult", player):
                    possible_plants.append(PlantData("Sprout-pult", 300, 0, 300, ["Puff-shroom", "Cabbage-pult"], []))
                break


            if state.has("Threepeater", player) and state.has("Jalapeno", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Phoenix Threepeater", 2000, 700, 800, ["Threepeater", "Jalapeno"], ["straight_shooter"]))

            if state.has("Plantern", player) and state.has("Cactus", player) and state.has("Umbrella Leaf", player) and state.has("Plant Gloves", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Pharos Umbrella", 2000, 2000, 2000, ["Plantern", "Cactus","Umbrella Leaf"], []))

            if state.has("Burger Blaster", player) and state.has("Cactus", player) and state.has("Melon-pult", player) and state.has("Ice-shroom", player) and state.has("Plant Gloves", player):  # with sunflower
                possible_plants.append(PlantData("Burger Blaster", 2000, 0, 1000, ["Burger Blaster", "Cactus","Melon-pult","Ice-shroom"], ["straight_shooter"]))
            if state.has("Snipea", player) and state.has("Doom-shroom", player) and "odyssey" in modifier_flags:  # with sunflower
                possible_plants.append(PlantData("Reaper Snipea", 2000, 700, 2000, ["Snipea", "Doom-shroom"], []))

            if state.has("Melon-pult", player) and state.has("Kernel-pult", player) and state.has("Cabbage-pult", player) and state.has("Garlic", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Wither-pult", 2000, 0, 2000, ["Melon-pult", "Kernel-pult","Cabbage-pult","Garlic"], []))

            if state.has("Melon-pult", player) and state.has("Ice-shroom", player) and state.has("Magnet-shroom", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Maelonstrom", 2000, 0, 2000, ["Melon-pult", "Ice-shroom","Magnet-shroom"], ["applies_cryo"]))


            if state.has("Chomper", player) and state.has("Wall-nut", player) and state.has("Cherry Bomb", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Cherrizilla", 2000, 0, 2000, ["Chomper", "Cherry Bomb","Wall-nut"], []))
            if state.has("Fume-shroom", player) and state.has("Hypno-shroom", player) and state.has("Ice-shroom", player)and state.has("Doom-shroom", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Doominator", 2000, 0, 2000, ["Fume-shroom", "Hypno-shroom","Ice-shroom","Doom-shroom"], []))

            if state.has("Spruce Sharpshooter", player) and state.has("Aloe Aqua", player) and state.has("Saw-me-not", player)and state.has("Snow Lotus", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Lifereaver Spruce", 2000, 0, 2000, ["Spruce Sharpshooter", "Aloe Aqua","Saw-me-not","Snow Lotus"], ["applies_cryo"]))






            if state.has("Starfruit", player) and state.has("Plantern", player) and state.has("Magnet-shroom", player)and state.has("Cactus", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Magnetar", 1800, 1000, 1000, ["Starfruit", "Plantern","Magnet-shroom","Cactus"], []))
            if state.has("Burger Blaster", player) and state.has("Cactus", player) and state.has("Melon-pult", player):  # with sunflower
                possible_plants.append(PlantData("Burger Blaster", 1800, 0, 900, ["Burger Blaster", "Cactus", "Melon-pult"],["straight_shooter"]))
            if state.has("Icetip Lily", player):
                possible_plants.append(PlantData("Icetip Lily", 1600, 1400, 600, ["Icetip Lily"], ["applies_cryo"]))

            if state.has("Peashooter", player) and state.has("Cherry Bomb", player) and state.has("Plant Gloves", player) and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Gatling Cherry", 1600, 0, 600, ["Peashooter","Cherry Bomb"], ["straight_shooter"]))

            if state.has("Sunflower", player) and state.has("Cabbage-pult", player) and state.has("Marigold", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Helios Cabbage", 1600, 0, 1600, ["Cabbage-pult", "Marigold"], []))

            if state.has("Peashooter", player) and state.has("Doom-shroom", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Gatling Doom", 1600, 0, 600, ["Peashooter","Doom-shroom"], ["straight_shooter"]))

            if state.has("Kernel-pult", player) and state.has("Puff-shroom", player) and state.has("Magnet-shroom", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Doominator", 1600, 1000, 1600, ["Kernel-pult", "Puff-shroom","Magnet-shroom"], []))

            if state.has("Peashooter", player) and state.has("Wall-nut", player) and state.has("Chomper", player):
                possible_plants.append(PlantData("Chompzilla", 1500, 0, 1500, ["Peashooter", "Wall-nut","Chomper"], []))

            if state.has("Fume-shroom", player) and state.has("Hypno-shroom", player) and state.has("Scaredy-shroom", player)and state.has("Magnet-shroom", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Charmitron", 1400, 0, 1400, ["Fume-shroom", "Hypno-shroom","Scaredy-shroom","Magnet-shroom"], []))

            if state.has("Snipea", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Snipea", 1400, 1400, 1400, ["Snipea"], []))

            if state.has("Peashooter", player) and state.has("Magnet-shroom", player) and state.has("Plantern", player) and state.has("Plant Gloves", player)and "upgrade_odyssey" in modifier_flags:
                possible_plants.append(PlantData("Photon Splitter", 1400, 0, 1400, ["Peashooter", "Plantern","Magnet-shroom"], []))


            if state.has("Peashooter", player) and state.has("Puff-shroom", player) and state.has("Magnet-shroom", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Buck-shroom Squad", 1300, 700, 800, ["Peashooter", "Puff-shroom","Magnet-shroom"], []))


            if state.has("Cabbage-pult", player) and state.has("Kernel-pult", player) and state.has("Melon-pult", player):
                possible_plants.append(PlantData("Salad-pult", 1300, 0, 600, ["Cabbage-pult","Kernel-pult","Melon-pult"], []))
            if state.has("Scaredy-shroom", player) and state.has("Fume-shroom", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Charm-shroom", 1200, 0, 1000, ["Scaredy-shroom", "Fume-shroom","Hypno-shroom"], ["straight_shooter"]))
            if state.has("Hypno-shroom", player) and "odyssey" in modifier_flags:
                possible_plants.append(PlantData("Empress-shroom", 1200, 0, 1200, ["Hypno-shroom"], []))
            if state.has("Starfruit", player) and state.has("Plantern", player) and state.has("Magnet-shroom", player):
                possible_plants.append(PlantData("Stardrop", 1200, 1100, 700, ["Starfruit", "Plantern","Magnet-shroom"], []))
            if state.has("Swordmaster Starfruit", player):
                possible_plants.append(PlantData("Swordmaster Starfruit", 1200, 1200, 600, ["Swordmaster Starfruit"], []))
            if state.has("Melon-pult", player) and state.has("Kernel-pult", player) and state.has("Jicamagic", player):
                possible_plants.append(PlantData("Melon Mortar", 1200, 1200, 1200, ["Melon-pult","Kernel-pult","Jicamagic"], ["scuba_no_lilypad","doesnt_freeze"]))
            if state.has("Amp-nion", player):
                possible_plants.append(PlantData("Ampnion", 1200, 1200, 1200, ["Amp-nion"], []))

            if state.has("Peashooter", player) and state.has("Puff-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Icicle-shroom", 1200, 0, 400, ["Peashooter", "Puff-shroom", "Ice-shroom"],["straight_shooter", "applies_cryo"]))

            if state.has("Kernel-pult", player) and state.has("Marigold", player):
                possible_plants.append(PlantData("Golden Kernel", 1200, 1200, 1200, ["Kernel-pult","Marigold"], []))
            if state.has("Fume-shroom", player) and state.has("Doom-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Doomspike-shroom", 1200, 0, 800, ["Fume-shroom","Doom-shroom","Ice-shroom"], ["applies_cryo"]))
            if state.has("Saw-me-not", player):
                possible_plants.append(PlantData("Twin Saw-me-not", 1000, 1000, 1000, ["Saw-me-not"], []))

            if state.has("Kernel-pult", player) and (state.has("Cob Cannon", player) or state.has("Fertilizer", player)):
                possible_plants.append(PlantData("Cob Cannon", 1000, 1000, 1000, ["Kernel-pult", "Cob Cannon"],["from_fertilizer","scuba_no_lilypad","doesnt_freeze"]))
            if state.has("Wall-nut", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Hypno Nut", 1000, 0, 1000, ["Wall-nut", "Hypno-shroom"],[]))
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
            if state.has("Threepeater", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Stench Spreader", 800, 250, 300, ["Threepeater","Garlic"], ["straight_shooter"]))
            if state.has("Kernel-pult", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Clove-pult", 800, 0, 800, ["Kernel-pult","Garlic"], []))
            if state.has("Melon-pult", player) and state.has("Fume-shroom", player):
                possible_plants.append(PlantData("Spring Melon", 800, 0, 800, ["Melon-pult","Fume-shroom"], []))
            if state.has("Peashooter", player) and state.has("Jalapeno", player):
                possible_plants.append(PlantData("Gatling Blaze", 800, 0, 400, ["Peashooter", "Jalapeno"], ["straight_shooter"]))
            if state.has("Peashooter", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Gatling Snow", 800, 0, 400, ["Peashooter", "Ice-shroom"], ["applies_cryo","straight_shooter"]))
            if state.has("Peashooter", player) and state.has("Cherry Bomb", player):
                possible_plants.append(PlantData("Cherry Gatling", 800, 0, 400, ["Peashooter","Cherry Bomb"], ["straight_shooter"]))
            if state.has("Threepeater", player) and state.has("Squash", player):
                possible_plants.append(PlantData("Squash-spreader", 800, 800, 400, ["Threepeater", "Squash"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Perfume-shroom", 800,  0, 800, ["Fume-shroom", "Hypno-shroom"], []))
            if state.has("Peashooter", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Gatling Posion", 800, 0, 800, ["Peashooter", "Garlic"], ["straight_shooter"]))
            if state.has("Melon-pult", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Hypno Melon", 750,  0, 750, ["Fume-shroom", "Hypno-shroom"], []))
            if state.has("Peashooter", player) and state.has("Hypno-shroom", player):
                possible_plants.append(PlantData("Hypno Pea", 750,  0, 300, ["Peashooter", "Hypno-shroom"], ["straight_shooter"]))

            if state.has("Starfruit", player) and state.has("Blover", player):
                possible_plants.append(PlantData("Star Blover", 750, 750, 375, ["Starfruit", "Blover"], []))
            if state.has("Fume-shroom", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Soot-shroom", 750, 0, 750, ["Fume-shroom", "Doom-shroom"], []))
            if state.has("Fume-shroom", player) and state.has("Garlic", player):
                possible_plants.append(PlantData("Foul-shroom", 700, 0, 700, ["Fume-shroom", "Garlic"], []))
            if state.has("Cabbage-pult", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Coldslaw", 700, 0, 700, ["Cabbage-pult", "Ice-shroom"], []))




            if state.has("Cabbage-pult", player) and state.has("Jicamagic", player):
                possible_plants.append(PlantData("Cab-barrage", 600, 600, 600, ["Cabbage-pult", "Jicamagic"], ["scuba_no_lilypad"]))
            if state.has("Fume-shroom", player) and state.has("Starfruit", player):
                possible_plants.append(PlantData("Starburst-shroom", 600, 0, 600, ["Fume-shroom", "Starfruit"], []))


            if state.has("Threepeater", player) and state.has("Peashooter", player):
                possible_plants.append(PlantData("Multipeater", 600, 300, 300, ["Threepeater", "Peashooter"], ["straight_shooter","torchwood_usable"]))
            if state.has("Threepeater", player) and state.has("Cherry Bomb", player):
                possible_plants.append(PlantData("Cherry Spreader", 600, 200, 300, ["Threepeater", "Cherry Bomb"], ["straight_shooter"]))
            if state.has("Scaredy-shroom", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Frenzy-shroom", 600, 0, 300, ["Scaredy-shroom", "Doom-shroom"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Frost-shroom", 600, 0, 600, ["Fume-shroom", "Ice-shroom"], ["applies_cryo"]))
            if state.has("Threepeater", player) and state.has("Jalapeno", player):
                possible_plants.append(PlantData("Scorched Threepeater", 600, 200, 300, ["Threepeater","Jalapeno"], ["straight_shooter"]))
            if state.has("Melon-pult", player) and state.has("Cabbage-pult", player):
                possible_plants.append(PlantData("Cracked Melon", 600, 0, 600, ["Melon-pult", "Cabbage-pult"], []))
            if state.has("Puff-shroom", player) and state.has("Melon-pult", player):
                possible_plants.append(PlantData("Slice-pult", 600, 0, 600, ["Melon-pult", "Puff-shroom"], []))
            if state.has("Cactus", player) and state.has("Plantern", player):
                possible_plants.append(PlantData("Lumos Cactus", 500, 0, 250, ["Cactus","Plantern"], ["targets_air"]))
            if state.has("Melon-Pult", player) and state.has("Squash", player):
                possible_plants.append(PlantData("Gourd-pult", 500, 0, 250, ["Melon-Pult","Squash"], []))
            if state.has("Peashooter", player) and state.has("Plantern", player):
                possible_plants.append(PlantData("Gatling Beam", 500, 0, 500, ["Peashooter","Plantern"], []))


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
            if state.has("Puff-shroom", player) and state.has("Starfruit", player):
                possible_plants.append(PlantData("Twinkle-shroom", 500, 0, 500, ["Starfruit", "Puff-shroom"], []))
            if state.has("Puff-shroom", player) and state.has("Kernel-pult", player):
                possible_plants.append(PlantData("Popcorn-pult", 500, 0, 500, ["Kernel-pult", "Puff-shroom"], []))

            if state.has("Scaredy-shroom", player) and state.has("Cherry Bomb", player):
                possible_plants.append(PlantData("Blasty-shroom", 400, 0, 200, ["Scaredy-shroom","Cherry Bomb"], ["straight_shooter"]))

            if state.has("Peashooter", player):
                possible_plants.append(PlantData("Gatling Pea", 400, 0, 200, ["Peashooter"], ["torchwood_usable","straight_shooter"]))
            if state.has("Scaredy-shroom", player) and state.has("Fume-shroom", player):
                possible_plants.append(PlantData("Gutsy-shroom", 350, 0, 175, ["Scaredy-shroom","Fume-shroom"], []))
            if state.has("Scaredy-shroom", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Shivery-shroom", 300, 0, 150, ["Scaredy-shroom","Ice-shroom"], ["applies_cryo","straight_shooter"]))
            if state.has("Threepeater", player):
                possible_plants.append(PlantData("Threepeater", 300, 100, 150, ["Threepeater"], ["torchwood_usable","straight_shooter"]))
            if state.has("Threepeater", player) and state.has("Plantern", player):
                possible_plants.append(PlantData("Ray-shroom", 300, 0, 300, ["Fume-shroom","Plantern"], []))

            if state.has("Peashooter", player) and state.has("Doom-shroom", player):
                possible_plants.append(PlantData("Doom Pea", 300, 0, 150, ["Peashooter","Doom-shroom"], ["straight_shooter"]))
            if state.has("Fume-shroom", player) and state.has("Magnet-shroom", player):
                possible_plants.append(PlantData("Morph-shroom", 300, 0, 300, ["Fume-shroom","Magnet-shroom"], []))
            if state.has("Bamblock", player) and state.has("Spruce Sharpshooter", player):
                possible_plants.append(PlantData("Bamboo Shooter", 300, 0, 150, ["Bamblock", "Spruce Sharpshooter"], []))
            if state.has("Puff-shroom", player) and state.has("Cabbage-pult", player):
                possible_plants.append(PlantData("Sprout-pult", 300, 0, 300, ["Puff-shroom", "Cabbage-pult"], []))
            if state.has("Squash", player) and state.has("Kernel-pult", player):
                possible_plants.append(PlantData("Butternut-pult", 300, 0, 300, ["Squash", "Kernel-pult"], []))
            if state.has("Cabbage-pult", player) and state.has("Squash", player):
                possible_plants.append(PlantData("Lettuce-pult", 250, 0, 250, ["Cabbage-pult","Squash"], []))

            if state.has("Fume-shroom", player) and state.has("Potato Mine", player):
                possible_plants.append(PlantData("Starch-shroom", 250, 0, 250, ["Fume-shroom","Potato Mine"], []))
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
            if state.has("Kernel-pult", player) and state.has("Ice-shroom", player):
                possible_plants.append(PlantData("Cobsicle", 150, 0, 150, ["Kernel-pult","Ice-shroom"], []))
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
            if state.has("Cabbage-pult", player):
                possible_plants.append(PlantData("Cabbage-pult", 100, 0, 100, ["Cabbage-pult"], []))
            if state.has("Scaredy-shroom", player):
                possible_plants.append(PlantData("Scaredy-shroom",100,0,40,["Scaredy-shroom"],["straight_shooter"]))
            if state.has("Kernel-pult", player):
                possible_plants.append(PlantData("Kernel-pult", 75, 0, 75, ["Kernel-pult"], []))
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
                pool_plant = None
                for i in possible_plants:
                    if i.pool_power > 0:
                        if "scuba" in modifier_flags and not (state.has("Lily Pad",player) or state.has("Sea-shroom",player) or state.has("Tangle Kelp",player)):
                            if "scuba_no_lilypad" not in i.special_tags:
                                continue
                        if "large_water" in modifier_flags:
                            if i.pool_power != i.power:
                                continue
                        pool_plant = i
                        break
                    else:
                        continue
                if pool_plant is None:
                    if "scuba" in modifier_flags:
                        return False
                    pool_plant = PlantData("None",0,0,0,[],[]) #no lily pad and no pool attackers
                #if pool_plant already clears level strength then skip
                if pool_plant.pool_power > level_strength:
                    for i in pool_plant.required_plants:
                        final_plants.append(i)
                else:
                    pool_bonus = 0
                    while True:
                        if state.has("Sea-shroom",player) and state.has("Cactus",player):
                            final_plants.append("Sea-shroom")
                            final_plants.append("Cactus")
                            pool_bonus = 400
                            break
                        elif state.has("Threepeater",player) and state.has("Tangle Kelp",player):
                            final_plants.append("Threepeater")
                            final_plants.append("Tangle Kelp")
                            pool_bonus = 250
                            break
                        elif state.has("Sea-shroom",player) and state.has("Fume-shroom",player):
                            final_plants.append("Sea-shroom")
                            final_plants.append("Fume-shroom")
                            pool_bonus = 150
                            break
                        elif state.has("Sea-shroom",player) and state.has("Puff-shroom",player):
                            final_plants.append("Sea-shroom")
                            final_plants.append("Puff-shroom")
                            pool_bonus = 150
                            break
                        elif state.has("Sea-shroom",player) and state.has("Starfruit",player):
                            final_plants.append("Sea-shroom")
                            final_plants.append("Starfruit")
                            pool_bonus = 100
                            break
                        elif state.has("Sea-shroom",player):
                            final_plants.append("Sea-shroom")
                            pool_bonus = 50
                            break
                        return False
                    if pool_bonus < level_strength:
                        if pool_bonus + pool_plant.pool_power >= level_strength:
                            for i in pool_plant.required_plants:
                                final_plants.append(i)
                        else:
                            return False

                #sea shroom 50
                #urchin-shroom 400
                #sea star 100
                # kelp spreader 250

                #bonus for kelp


                #check for pool power
                #do this by both checking possible_plants and things like sea shroom
                #if one is greater than pool power then use that
                #if both are greater than pool power then pick the one with the least seed slots needed
                #if neither then check if both combined can do it, and if not immediately return false

                #if scuba flag and selected plant cant deal with it, pick again
                #last resort, try to fit a scuba counter alongside a stronger attacker


                    # if pool plant power is 0 then check for sea-shroom, then tangle kelp



        #blasty-shroom 400 min
        #sea nut




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






        if "diamond_box" in modifier_flags:
            if state.has("Chomper", player):
                final_plants.append("Chomper")
            else:
                level_strength = 800

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

        if state.has("Torchwood", player):
            for i in possible_plants:
                if "torchwood_usable" in i.special_tags:
                    i.power += i.power

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
        #todo night logic here
        cpl = 200
        least_cpl = 200
        if "night" in modifier_flags:
            for i in final_plants:
                if i == "Peashooter":
                    cpl = 100
                if i == "Wall-nut":
                    cpl = 50
                if i == "Potato Mine":
                    cpl = 25
                if i == "Puff-shroom":
                    cpl = 0
                if i == "Fume-shroom":
                    cpl = 75
                if i == "Hypno-shroom":
                    cpl = 75
                if i == "Scaredy-shroom":
                    cpl = 25
                if i == "Doom-shroom":
                    cpl = 50
                if i == "Squash":
                    cpl = 50
                if i == "Threepeater":
                    cpl = 100
                if i == "Sea-shroom" and "water" in modifier_flags:
                    cpl = 0
                if i == "Starfruit":
                    cpl = 100
                if i == "Cabbage-pult":
                    cpl = 100
                if i == "Kernel-pult":
                    cpl = 100
                if i == "Bamblock":
                    cpl = 50
                if i == "Nyan Squash":
                    cpl = 50
                if i == "Lily Pad" and (state.has("Fertilizer",player)) and "water"in modifier_flags:
                    cpl = 25
                if i == "Cattail" and "water" in modifier_flags:#cattail should never be chosen without lily pad so this is fine
                    cpl = 25
                if i == "Cattail Girl" and "water" in modifier_flags:
                    cpl = 25
                least_cpl = min(least_cpl,cpl)
            if level_strength < 250:
                cpl_requirement = 100
            elif level_strength < 350:
                cpl_requirement = 75
            elif level_strength < 550:
                cpl_requirement = 50
            else:
                cpl_requirement = 25
            if least_cpl > cpl_requirement:#if you dont have a plant in your loadout that satisfies cpl then...
                if len(final_plants) > free_slots:
                    return False
                #add a plant that satisfies cpl
                cpl = 200
                if cpl_requirement<200:
                    if state.has("Peashooter", player):
                        final_plants.append("Peashooter")
                        cpl = 100
                    elif state.has("Starfruit", player):
                        final_plants.append("Starfruit")
                        cpl = 100
                    elif state.has("Cabbage-pult", player):
                        final_plants.append("Cabbage-pult")
                        cpl = 100
                    elif state.has("Kernel-pult", player):
                        final_plants.append("Kernel-pult")
                        cpl = 100
                    elif cpl_requirement<100:
                        if state.has("Fume-shroom", player):
                            final_plants.append("Fume-shroom")
                            cpl = 75
                        elif state.has("Hypno-shroom", player):
                            final_plants.append("Hypno-shroom")
                            cpl = 75
                        elif cpl_requirement<75:
                            if state.has("Wall-nut", player):
                                final_plants.append("Wall-nut")
                                cpl = 50
                            elif state.has("Doom-shroom", player):
                                final_plants.append("Doom-shroom")
                                cpl = 50
                            elif state.has("Squash", player):
                                final_plants.append("Squash")
                                cpl = 50
                            elif state.has("Bamblock", player):
                                final_plants.append("Bamblock")
                                cpl = 50
                            elif state.has("Nyan Squash", player):
                                final_plants.append("Nyan Squash")
                                cpl = 50
                            elif cpl<50:
                                if state.has("Potato Mine", player):
                                    final_plants.append("Potato Mine")
                                    cpl = 25
                                elif state.has("Puff-shroom", player):
                                    final_plants.append("Puff-shroom")
                                    cpl = 0
                                elif state.has("Scaredy-shroom", player):
                                    final_plants.append("Scaredy-shroom")
                                    cpl = 25
                                elif state.has("Sea-shroom", player) and "water" in modifier_flags:
                                    final_plants.append("Sea-shroom")
                                    cpl = 0
                                elif state.has("Lily Pad",player) and "water" in modifier_flags and state.has("Fertilizer",player):
                                    final_plants.append("Lily Pad")
                                    cpl = 0
                                elif len(final_plants) > free_slots+1 and state.has("Lily Pad",player) and "water" in modifier_flags and state.has("Cattail",player):
                                    final_plants.append("Cattail")
                                    final_plants.append("Lily Pad")
                                    cpl = 25
                                elif state.has("Cattail Girl",player) and "water" in modifier_flags:
                                    final_plants.append("Cattail Girl")
                                    cpl = 25
                if cpl < cpl_requirement:
                    if state.has("Lawnmowers",player) and "water" not in modifier_flags and cpl < 100:
                            pass
                    elif state.has("Lawnmowers",player) and state.has("Pool Cleaners",player) and "water" in modifier_flags and cpl < 100:
                            pass
                    else:
                        return False
                #if you cant then return false


            #calculate cpl requirement using level strength
            #200 -> cpl 100
            #350 -> cpl 50
            #600 -> cpl 25
            #caps at 25
        #go through final plants and make sure theres enough cheap plants and if not, add some (decide by cost per lane to defend)
        #include lawnmowers and pool cleaners
        #squash - 50
        #threepeater - 125
        #peashooter - 100
        #puff-shroom 0
        #fume 75
        #scaredy 25
        #lawnmowers count as 0, but require any other 'cheap' plant
        #anything not listed isnt considered
        # if sun cost isnt enough, average everything then divide it by number again (having multiple cheap plants helps



        while len(final_plants) < free_slots:
            if "compact_planting" in modifier_flags:
                if state.has("Doom-shroom", player):
                    if "Doom-shroom" not in final_plants:
                        final_plants.append("Doom-shroom")
                        level_strength -= 300
                        continue
                if state.has("Wall-nut", player):
                    if "Wall-nut" not in final_plants:
                        final_plants.append("Wall-nut")
                        level_strength -= 300
                        continue
                if state.has("Hypno-shroom", player):
                    if "Hypno-shroom" not in final_plants:
                        final_plants.append("Hypno-shroom")
                        level_strength -= 100
                        continue
                if state.has("Ice-shroom", player):
                    if "Ice-shroom" not in final_plants:
                        final_plants.append("Ice-shroom")
                        level_strength -= 100
                        continue
                if state.has("Ice-shroom", player):
                    if "Ice-shroom" not in final_plants:
                        final_plants.append("Ice-shroom")
                        level_strength -= 100
                        continue
                if state.has("Potato Mine", player):
                    if "Potato Mine" not in final_plants:
                        final_plants.append("Potato Mine")
                        level_strength -= 100
                        continue
                break
            else:
                if state.has("Doom-shroom", player):#todo add snow lotus if its a snow level or snowball launchers
                    if "Doom-shroom" not in final_plants:
                        final_plants.append("Doom-shroom")
                        level_strength -= 400
                        continue
                if state.has("Pumpkin", player):
                    if "Pumpkin" not in final_plants:
                        final_plants.append("Pumpkin")
                        level_strength -= 300
                        continue
                if state.has("Wall-nut", player):
                    if "Wall-nut" not in final_plants:
                        final_plants.append("Wall-nut")
                        level_strength -= 300
                        continue
                if state.has("Cherry Bomb", player):
                    if "Cherry Bomb" not in final_plants:
                        final_plants.append("Cherry Bomb")
                        level_strength -= 200
                        continue
                if state.has("Chomper", player):
                    if "Chomper" not in final_plants:
                        final_plants.append("Chomper")
                        level_strength -= 200
                        continue
                if state.has("Jalapeno", player):
                    if "Jalapeno" not in final_plants:
                        final_plants.append("Jalapeno")
                        level_strength -= 150
                        continue
                if state.has("Bamblock", player):
                    if "Bamblock" not in final_plants:
                        final_plants.append("Bamblock")
                        level_strength -= 150
                        continue
                if state.has("Nyan Squash", player):
                    if "Nyan Squash" not in final_plants:
                        final_plants.append("Nyan Squash")
                        level_strength -= 150
                        continue
                if state.has("Hypno-shroom", player):
                    if "Hypno-shroom" not in final_plants:
                        final_plants.append("Hypno-shroom")
                        level_strength -= 150
                        continue
                if state.has("Spikeweed", player):
                    if "Spikeweed" not in final_plants:
                        final_plants.append("Spikeweed")
                        level_strength -= 100
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
        #Snipea = 0 (temp)
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
    if options.adventure_extra==2:
        connect_regions(world, player, "Menu", "Snow", lambda state: state.has("Snow Access", player))
    if options.challenge_sanity:
        connect_regions(world, player, "Menu", "Fusion Challenges", lambda state: state.has("Fusion Challenge Access", player))
    #if options.showcase_sanity:
    #    connect_regions(world, player, "Menu", "Fusion Showcase", lambda state: state.has("Fusion Showcase Access", player))
    if options.minigame_sanity:
        connect_regions(world, player, "Menu", "Minigames", lambda state: True)

    if options.vasebreaker_sanity:
        connect_regions(world, player, "Menu", "Vasebreaker", lambda state: state.has("Vasebreaker Access", player))

    if options.survival_sanity:
        connect_regions(world, player, "Menu", "Survival", lambda state: True)

    if options.adventure_odyssey or options.goal_type == 2:
        connect_regions(world, player, "Menu", "Odyssey Menu",  lambda state: state.has("Odyssey Key", player))

    if options.odyssey_minigames!= 0:
        connect_regions(world, player, "Menu", "Odyssey Minigames", lambda state: state.has("Odyssey Key", player))


    cherry_newspaper = "CHER & WALL | CHER & PUMPK | CHER & MEL | CHER & PEA & GLOV"
    base_pool = "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT"
    base_snow = "FIR | CHER | DOOM | ICE | JAL"
    balloon_pool = "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT" #the way i understand how rf makes rules means i can't combine different strings
    # parenthesis aren't supported by rf so
    #though adding different rules to the same l ocation might work depending on how thats handled
    add_rule(world.get_location("Day: Level 2 (1)", player),
             lambda state: can_beat_power_level(state, 100,[],0))
    add_rule(world.get_location("Day: Level 2 (2)", player),
             lambda state: can_beat_power_level(state, 100,[],0))

    add_rule(world.get_location("Day: Level 3 (1)", player),
             lambda state: can_beat_power_level(state, 200,[],0))
    add_rule(world.get_location("Day: Level 3 (2)", player),
             lambda state: can_beat_power_level(state, 200,[],0))
    add_rule(world.get_location("Day: Level 4 (1)", player),
             lambda state: can_beat_power_level(state, 150,[],0))
    add_rule(world.get_location("Day: Level 4 (2)", player),
             lambda state: can_beat_power_level(state, 150,[],0))
    add_rule(world.get_location("Day: Level 5 (1)", player),
             lambda state: can_beat_power_level(state, 200,[],0))
    add_rule(world.get_location("Day: Level 5 (2)", player),
             lambda state: can_beat_power_level(state, 200,[],0))

    add_rule(world.get_location("Day: Level 6 (1)", player),
             lambda state: can_beat_power_level(state, 200,["diamond_box"],0))#chomperma
    add_rule(world.get_location("Day: Level 6 (2)", player),
             lambda state: can_beat_power_level(state, 200,["diamond_box"],0))

    add_rule(world.get_location("Day: Level 7 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["cherry_newspaper","diamond_box"],0))  # chomperma
    add_rule(world.get_location("Day: Level 7 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["cherry_newspaper","diamond_box"],0))

    add_rule(world.get_location("Day: Level 8 (1)", player),
             lambda state: can_beat_power_level(state, 400,[],0))
    add_rule(world.get_location("Day: Level 8 (2)", player),
             lambda state: can_beat_power_level(state, 400,[],0))

    add_rule(world.get_location("Day: Level 9 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["cherry_newspaper","diamond_box"],0))  # chomperma and cherry nut
    add_rule(world.get_location("Day: Level 9 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["cherry_newspaper","diamond_box"],0))

    add_rule(world.get_location("Night: Level 11 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["night"],0))  # screen door
    add_rule(world.get_location("Night: Level 11 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["night"],0))

    add_rule(world.get_location("Night: Level 12 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 12 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 13 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 13 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 14 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 14 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 15 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0)) #dancing vaulter
    add_rule(world.get_location("Night: Level 15 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 16 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0)) #dancing vaulter +
    add_rule(world.get_location("Night: Level 16 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["night"],0))
    add_rule(world.get_location("Night: Level 17 (1)", player),
             lambda state: can_beat_power_level(state, 600, ["night"],0)) #dancing vaulter +
    add_rule(world.get_location("Night: Level 17 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["night"],0))
    add_rule(world.get_location("Night: Level 18 (1)", player),
             lambda state: can_beat_power_level(state, 600, ["diamond_box","night"],0)) #dancing vaulter +
    add_rule(world.get_location("Night: Level 18 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["diamond_box","night"],0))

    add_rule(world.get_location("Pool: Level 19 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water"],0))
    add_rule(world.get_location("Pool: Level 19 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water"],0))
    add_rule(world.get_location("Pool: Level 20 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["water"],0))
    add_rule(world.get_location("Pool: Level 20 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["water"],0))
    add_rule(world.get_location("Pool: Level 21 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","scuba"],0))
    add_rule(world.get_location("Pool: Level 21 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","scuba"],0))
    add_rule(world.get_location("Pool: Level 22 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water"],0))
    add_rule(world.get_location("Pool: Level 22 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water"],0))
    add_rule(world.get_location("Pool: Level 23 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water"],0))
    add_rule(world.get_location("Pool: Level 23 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water"],0))
    add_rule(world.get_location("Pool: Level 24 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))
    add_rule(world.get_location("Pool: Level 24 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))
    add_rule(world.get_location("Pool: Level 25 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))
    add_rule(world.get_location("Pool: Level 25 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))
    add_rule(world.get_location("Pool: Level 26 (1)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))
    add_rule(world.get_location("Pool: Level 26 (2)", player),
             lambda state: can_beat_power_level(state, 300, ["water"],0))

    add_rule(world.get_location("Fog: Level 28 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","night"],0))
    add_rule(world.get_location("Fog: Level 28 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","night"],0))
    add_rule(world.get_location("Fog: Level 29 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","night"],0))
    add_rule(world.get_location("Fog: Level 29 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","night"],0))
    add_rule(world.get_location("Fog: Level 30 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","night"],0))
    add_rule(world.get_location("Fog: Level 30 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","night"],0))
    add_rule(world.get_location("Fog: Level 31 (1)", player),
             lambda state: can_beat_power_level(state, 350, ["water","balloon","night"],0))#screen door
    add_rule(world.get_location("Fog: Level 31 (2)", player),
             lambda state: can_beat_power_level(state, 350, ["water","balloon","night"],0))
    add_rule(world.get_location("Fog: Level 32 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon","tough_balloon","night"],0))#screen door
    add_rule(world.get_location("Fog: Level 32 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon","tough_balloon","night"],0))
    add_rule(world.get_location("Fog: Level 33 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger","night"],0))
    add_rule(world.get_location("Fog: Level 33 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["water","balloon","digger","night"],0))
    add_rule(world.get_location("Fog: Level 34 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon","digger","night"],0))
    add_rule(world.get_location("Fog: Level 34 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["water","balloon","digger","night"],0))
    add_rule(world.get_location("Fog: Level 35 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["water","balloon","tough_balloon","night"],0))#clown in the box
    add_rule(world.get_location("Fog: Level 35 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["water","balloon","tough_balloon","night"],0))

    add_rule(world.get_location("Roof: Level 37 (1)", player),
             lambda state: can_beat_power_level(state, 100, ["roof","sloped","6_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 37 (2)", player),
             lambda state: can_beat_power_level(state, 100, ["roof","sloped","6_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 38 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 38 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 39 (1)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 39 (2)", player),
             lambda state: can_beat_power_level(state, 200, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 40 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 40 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 41 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","mecha_nut"],0)) #bungee + catapult
    add_rule(world.get_location("Roof: Level 41 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","mecha_nut"],0))
    add_rule(world.get_location("Roof: Level 42 (1)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","melon_pogo"],0)) #catapult
    add_rule(world.get_location("Roof: Level 42 (2)", player),
             lambda state: can_beat_power_level(state, 400, ["roof","sloped","3_starting_pots","melon_pogo"],0))
    add_rule(world.get_location("Roof: Level 43 (1)", player),
             lambda state: can_beat_power_level(state, 500, ["roof", "sloped", "3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 43 (2)", player),
             lambda state: can_beat_power_level(state, 500, ["roof", "sloped", "3_starting_pots"],0))
    add_rule(world.get_location("Roof: Level 44 (1)", player),
             lambda state: can_beat_power_level(state, 600, ["roof", "sloped", "3_starting_pots"],0))  # cherry catapult
    add_rule(world.get_location("Roof: Level 44 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["roof", "sloped", "3_starting_pots"],0))
    if options.adventure_extra == 2:

        add_rule(world.get_location("Snow: Level 1 (1)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow"],0))
        add_rule(world.get_location("Snow: Level 1 (2)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow"],0))
        add_rule(world.get_location("Snow: Level 2 (1)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer"],0))
        add_rule(world.get_location("Snow: Level 2 (2)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer"],0))
        add_rule(world.get_location("Snow: Level 3 (1)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer","long_snow"],0))
        add_rule(world.get_location("Snow: Level 3 (2)", player),
                 lambda state: can_beat_power_level(state, 150, ["snow","shieldbearer","long_snow"],0))
        add_rule(world.get_location("Snow: Level 4 (1)", player),
                 lambda state: can_beat_power_level(state, 250, ["snow","shieldbearer","long_snow","snowball_launcher"],0))
        add_rule(world.get_location("Snow: Level 4 (2)", player),
                 lambda state: can_beat_power_level(state, 250, ["snow","shieldbearer","long_snow","snowball_launcher"],0))
        add_rule(world.get_location("Snow: Level 5 (1)", player),
                 lambda state: can_beat_power_level(state, 350, ["snow","shieldbearer","long_snow","snowball_launcher"],0))
        add_rule(world.get_location("Snow: Level 5 (2)", player),
                 lambda state: can_beat_power_level(state, 350, ["snow","shieldbearer","long_snow","snowball_launcher"],0))
        add_rule(world.get_location("Snow: Level 6 (1)", player),
                 lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher", "furling"],0))#yeti
        add_rule(world.get_location("Snow: Level 6 (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher", "furling"],0))
        add_rule(world.get_location("Snow: Level 7 (1)", player),
                 lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher"],0))#yeti
        add_rule(world.get_location("Snow: Level 7 (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["snow","shieldbearer","long_snow","snowball_launcher"],0))



        add_rule(world.get_location("Snow: Level 8 (1)", player),
                 lambda state: can_beat_power_level(state, 600,["snow", "shieldbearer", "long_snow", "furling"],0))  # yeti
        add_rule(world.get_location("Snow: Level 8 (2)", player),
             lambda state: can_beat_power_level(state, 600, ["snow", "shieldbearer", "long_snow", "furling"],0))

        add_rule(world.get_location("Snow: Level 9 (1)", player),
                 lambda state: can_beat_conveyor(state, ["Bamblock","Firnace","Spruce Sharpshooter","Aloe Aqua","Snow Lotus","Saw-me-not"], [], 0))
        add_rule(world.get_location("Snow: Level 9 (2)", player),
                 lambda state: can_beat_conveyor(state, ["Bamblock","Firnace","Spruce Sharpshooter","Aloe Aqua","Snow Lotus","Saw-me-not"], [], 0))

#cattail girl carries without sunflowers btw

    add_rule(world.get_location("Pool: Level 27 (1)", player),
        lambda state: can_beat_conveyor(state,["Threepeater","Lily Pad"],["Squash","Wall-nut","Spikeweed","Torchwood","Jalapeno"],0.60))
    add_rule(world.get_location("Pool: Level 27 (2)", player),
        lambda state: can_beat_conveyor(state, ["Threepeater", "Lily Pad"],["Squash", "Wall-nut", "Spikeweed", "Torchwood", "Jalapeno"], 0.60))

    add_rule(world.get_location("Fog: Level 36 (1)", player),
             lambda state: can_beat_conveyor(state, ["Lily Pad"], ["Plantern", "Starfruit", "Magnet-shroom", "Pumpkin", "Sea-shroom"], 0.80))
    add_rule(world.get_location("Fog: Level 36 (1)", player),lambda state: (state.has("Cactus",player) or state.has("Fertilizer",player)))

    add_rule(world.get_location("Fog: Level 36 (2)", player),
             lambda state: can_beat_conveyor(state, ["Lily Pad"], ["Plantern", "Starfruit", "Magnet-shroom", "Pumpkin", "Sea-shroom"], 0.80))
    add_rule(world.get_location("Fog: Level 36 (2)", player),lambda state: (state.has("Cactus",player) or state.has("Fertilizer",player)))

    add_rule(world.get_location("Roof: Level 45 (1)", player),
             lambda state: can_beat_conveyor(state, ["Flower Pot","Melon-pult"],["Kernel-pult", "Cabbage-pult", "Umbrella Leaf", "Ice-shroom", "Jalapeno"], 0.60))
    add_rule(world.get_location("Roof: Level 45 (2)", player),
             lambda state: can_beat_conveyor(state, ["Flower Pot", "Melon-pult"],["Kernel-pult", "Cabbage-pult", "Umbrella Leaf", "Ice-shroom", "Jalapeno"],0.60))






#fume shroom and melon pult have jalapeno fusions
#peashooter fume scaredy melon have ice shroom fusions
#spruce supershooter?



#
##snow levels require explosives or firnace btw
#

    if options.challenge_sanity:
        add_rule(world.get_location("Fusion Challenge: Explode-o-shooter (1)", player),
                 lambda state: can_beat_power_level(state, 300,["cherry_newspaper"],0))
        add_rule(world.get_location("Fusion Challenge: Explode-o-shooter (2)", player),
                 lambda state: can_beat_power_level(state, 300, ["cherry_newspaper"],0))
        add_rule(world.get_location("Fusion Challenge: Chompzilla (1)", player),
                 lambda state: can_beat_power_level(state, 200,["diamond_box"],0))
        add_rule(world.get_location("Fusion Challenge: Chompzilla (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["diamond_box"],0))
        add_rule(world.get_location("Fusion Challenge: Charm-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 600,["night"],0))
        add_rule(world.get_location("Fusion Challenge: Charm-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 600, ["night"],0))
        add_rule(world.get_location("Fusion Challenge: Doomspike-shroom (1)", player),
                 lambda state: can_beat_power_level(state, 600,["night"],0))
        add_rule(world.get_location("Fusion Challenge: Doomspike-shroom (2)", player),
                 lambda state: can_beat_power_level(state, 600, ["night"],0))
        add_rule(world.get_location("Fusion Challenge: Infernowood (1)", player),
                 lambda state: can_beat_power_level(state, 600,[],0))
        add_rule(world.get_location("Fusion Challenge: Infernowood (2)", player),
                 lambda state: can_beat_power_level(state, 600, [],0))
        add_rule(world.get_location("Fusion Challenge: Krakerberus (1)", player),
                 lambda state: can_beat_power_level(state, 200, ["water", "scuba"],0))
        add_rule(world.get_location("Fusion Challenge: Krakerberus (2)", player),
                 lambda state: can_beat_power_level(state, 200, ["water", "scuba"],0))
        add_rule(world.get_location("Fusion Challenge: Stardrop (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","cherry_newspaper","balloon","night"],0))
        add_rule(world.get_location("Fusion Challenge: Stardrop (2)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","cherry_newspaper","balloon","night"],0))
        add_rule(world.get_location("Fusion Challenge: Bloverthorn Pumpkin (1)", player),
                 lambda state: can_beat_power_level(state, 300, ["water","night"],0))
        add_rule(world.get_location("Fusion Challenge: Bloverthorn Pumpkin (2)", player),
                 lambda state: can_beat_power_level(state, 300, ["water","night"],0))
        add_rule(world.get_location("Fusion Challenge: Salad-pult (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped"],0))
        add_rule(world.get_location("Fusion Challenge: Salad-pult (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped"],0))
        add_rule(world.get_location("Fusion Challenge: Alchemist Umbrella (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped","mecha_nut"],0))
        add_rule(world.get_location("Fusion Challenge: Alchemist Umbrella (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof","3_starting_pots","sloped","mecha_nut"],0))
        add_rule(world.get_location("Fusion Challenge: Spruce Supershooter (1)", player),
                 lambda state: can_beat_power_level(state, 600, ["snow","cherry_newspaper","shieldbearer"],0))
        add_rule(world.get_location("Fusion Challenge: Spruce Supershooter (2)", player),
                 lambda state: can_beat_power_level(state, 600, ["snow","cherry_newspaper","shieldbearer"],0))
        add_rule(world.get_location("Fusion Challenge: Jicamagic (1)", player),
             lambda state: can_beat_power_level(state, 600, ["cherry_newspaper", "diamond_box"],0))
        add_rule(world.get_location("Fusion Challenge: Jicamagic (2)", player),
             lambda state: can_beat_power_level(state, 600, ["cherry_newspaper", "diamond_box"],0))
#    if False:
#        add_rule(world.get_location("Fusion Showcase: Pumpkin Bunker (1)", player),
#                 lambda state: can_beat_power_level(state, 400,["water","cherry_newspaper","balloon","diamond_box","tough_balloon"]))#more seed slots needed
#        add_rule(world.get_location("Fusion Showcase: Pumpkin Bunker (2)", player),
#                 lambda state: can_beat_power_level(state, 400, ["water","cherry_newspaper","balloon","diamond_box","tough_balloon"]))
#        add_rule(world.get_location("Fusion Showcase: Nugget-shroom (1)", player),
#                 lambda state: can_beat_power_level(state, 600,[]))
#        add_rule(world.get_location("Fusion Showcase: Nugget-shroom (2)", player),
#                 lambda state: can_beat_power_level(state, 600, []))
#        add_rule(world.get_location("Fusion Showcase: Spuddy-shroom (1)", player),
#                 lambda state: can_beat_power_level(state, 300,[]))
#        add_rule(world.get_location("Fusion Showcase: Spuddy-shroom (2)", player),
#                 lambda state: can_beat_power_level(state, 300, []))
#        add_rule(world.get_location("Fusion Showcase: Foul-shroom (1)", player),
#                 lambda state: can_beat_power_level(state, 800,[]))
#        add_rule(world.get_location("Fusion Showcase: Foul-shroom (2)", player),
#                 lambda state: can_beat_power_level(state, 800, []))
#        add_rule(world.get_location("Fusion Showcase: Boomwood (1)", player),
#                 lambda state: can_beat_power_level(state, 800,[]))
#        add_rule(world.get_location("Fusion Showcase: Boomwood (2)", player),
#                 lambda state: can_beat_power_level(state, 800, []))
#        add_rule(world.get_location("Fusion Showcase: Spike-nut (1)", player),
#                 lambda state: can_beat_power_level(state, 200,["water"]))
#        add_rule(world.get_location("Fusion Showcase: Spike-nut (2)", player),
#                 lambda state: can_beat_power_level(state, 200, ["water"]))
#        add_rule(world.get_location("Fusion Showcase: Leviathan-shroom (1)", player),
#                 lambda state: can_beat_power_level(state, 200,["water","scuba"]))#starting plants [sea-shroom, tangle kelp, jicamagic]
#        add_rule(world.get_location("Fusion Showcase: Leviathan-shroom (2)", player),
#                 lambda state: can_beat_power_level(state, 200, ["water","scuba"]))
#
#        rf.assign_rule("Fusion Showcase: Explod-o-tato Mine (1)","PEA+WALL+JAL+CHER+MINE")
#        rf.assign_rule("Fusion Showcase: Explod-o-tato Mine (2)","PEA+WALL+JAL+CHER+MINE")
#
#        rf.assign_rule("Fusion Showcase: Chomper Maw (1)", "JIQ+CHOM")#possible with only fused chompers
#        rf.assign_rule("Fusion Showcase: Chomper Maw (2)", "JIQ+CHOM")#possible with only fused chompers
#
#        rf.assign_rule("Fusion Showcase: Mind-blover (1)", "HYP+MAGN+BLOV")
#        rf.assign_rule("Fusion Showcase: Mind-blover (2)", "HYP+MAGN+BLOV")
#
#        rf.assign_rule("Fusion Showcase: Bamboom (1)", "CHER+JAL")
#        rf.assign_rule("Fusion Showcase: Bamboom (2)", "CHER+JAL")
#
#
#

    if options.survival_sanity:
        add_rule(world.get_location("Survival: Day (1)", player),
             lambda state: can_beat_power_level(state, 400, [], 0) and state.has("Survival Day", player))
        add_rule(world.get_location("Survival: Day (2)", player),
             lambda state: state.can_reach_location("Survival: Day (1)",player))
        add_rule(world.get_location("Survival: Day (Hard) (1)", player),
             lambda state: can_beat_power_level(state, 800, [], 0) and state.has("Survival Day", player))
        add_rule(world.get_location("Survival: Day (Hard) (2)", player),
             lambda state: state.can_reach_location("Survival: Day (Hard) (1)",player))

        add_rule(world.get_location("Survival: Night (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["night"], 0) and state.has("Survival Night", player))
        add_rule(world.get_location("Survival: Night (2)", player),
                 lambda state: state.can_reach_location("Survival: Night (1)", player))
        add_rule(world.get_location("Survival: Night (Hard) (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["night"], 0) and state.has("Survival Night", player))
        add_rule(world.get_location("Survival: Night (Hard) (2)", player),
                 lambda state: state.can_reach_location("Survival: Night (Hard) (1)", player))

        add_rule(world.get_location("Survival: Pool (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["water"], 0) and state.has("Survival Pool", player))
        add_rule(world.get_location("Survival: Pool (2)", player),
                 lambda state: state.can_reach_location("Survival: Pool (1)", player))
        add_rule(world.get_location("Survival: Pool (Hard) (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["water"], 0) and state.has("Survival Pool", player))
        add_rule(world.get_location("Survival: Pool (Hard) (2)", player),
                 lambda state: state.can_reach_location("Survival: Pool (Hard) (1)", player))

        add_rule(world.get_location("Survival: Fog (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["night","water"], 0) and state.has("Survival Fog", player))
        add_rule(world.get_location("Survival: Fog (2)", player),
                 lambda state: state.can_reach_location("Survival: Fog (1)", player))
        add_rule(world.get_location("Survival: Fog (Hard) (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["night","water"], 0) and state.has("Survival Fog", player))
        add_rule(world.get_location("Survival: Fog (Hard) (2)", player),
                 lambda state: state.can_reach_location("Survival: Fog (Hard) (1)", player))

        add_rule(world.get_location("Survival: Roof (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["roof", "3_starting_pots", "sloped"], 0) and state.has("Survival Roof", player))
        add_rule(world.get_location("Survival: Roof (2)", player),
                 lambda state: state.can_reach_location("Survival: Roof (1)", player))
        add_rule(world.get_location("Survival: Roof (Hard) (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["roof", "3_starting_pots", "sloped"], 0) and state.has("Survival Roof", player))
        add_rule(world.get_location("Survival: Roof (Hard) (2)", player),
                 lambda state: state.can_reach_location("Survival: Roof (Hard) (1)", player))



    if options.minigame_sanity>0:
        add_rule(world.get_location("Compact Planting (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["compact_planting"],0) and state.has("Compact Planting",player))
        add_rule(world.get_location("Compact Planting (2)", player),
                 lambda state: can_beat_power_level(state, 400, ["compact_planting"],0) and state.has("Compact Planting",player))
        add_rule(world.get_location("Newspaper War (1)", player),
             lambda state: can_beat_power_level(state, 300, ["cherry_newspaper","night"],0) and state.has("Newspaper War",player))
        add_rule(world.get_location("Newspaper War (2)", player),
             lambda state: can_beat_power_level(state, 300, ["cherry_newspaper","night"],0) and state.has("Newspaper War",player))
        add_rule(world.get_location("Matryoshka (1)", player),
             lambda state: can_beat_power_level(state, 800, ["diamond_box"],0) and state.has("Matryoshka",player))
        add_rule(world.get_location("Matryoshka (2)", player),
             lambda state: can_beat_power_level(state, 800, ["diamond_box"],0) and state.has("Matryoshka",player))
        add_rule(world.get_location("Pogo Party! (1)", player),
                 lambda state: can_beat_power_level(state, 200,["roof", "melon_pogo", "sloped", "3_starting_pots"],0) and state.has("Pogo Party!", player))
        add_rule(world.get_location("Pogo Party! (2)", player),
                 lambda state: can_beat_power_level(state, 200,["roof", "melon_pogo", "sloped", "3_starting_pots"],0) and state.has("Pogo Party!", player))
        add_rule(world.get_location("Bungee Blitz (1)", player),
             lambda state: state.has("Bungee Blitz", player))
        add_rule(world.get_location("Bungee Blitz (2)", player),
             lambda state: state.has("Bungee Blitz", player))
        rf.assign_rule("Bungee Blitz (1)", "POT+PUMPK+CHOM+CHER")#todo replace with standard conveyor logic
        rf.assign_rule("Bungee Blitz (2)", "POT+PUMPK+CHOM+CHER")
        add_rule(world.get_location("Beghouled (1)", player),
                 lambda state: state.has("Beghouled", player))
        add_rule(world.get_location("Beghouled (2)", player),
                 lambda state: state.has("Beghouled", player))
        add_rule(world.get_location("Seeing Stars (1)", player),
                 lambda state: state.has("Seeing Stars", player) and state.has("Starfruit", player)and state.has("Magnet-shroom", player)and state.has("Blover", player) and state.has("Plantern", player))
        add_rule(world.get_location("Seeing Stars (2)", player),
                 lambda state: state.has("Seeing Stars", player) and state.has("Starfruit", player)and state.has("Magnet-shroom", player)and state.has("Blover", player) and state.has("Plantern", player))


        add_rule(world.get_location("Wall-nut Billiards (1)", player),
                 lambda state: state.has("Wall-nut Billiards", player))
        add_rule(world.get_location("Wall-nut Billiards (2)", player),
                 lambda state: state.has("Wall-nut Billiards", player))
        add_rule(world.get_location("Whack a Zombie (1)", player),
                 lambda state: state.has("Whack a Zombie", player) and state.has("Mallet", player))
        add_rule(world.get_location("Whack a Zombie (2)", player),
                 lambda state: state.has("Whack a Zombie", player) and state.has("Mallet", player))
        add_rule(world.get_location("High Gravity (1)", player),
                 lambda state: state.has("High Gravity", player) and can_beat_power_level(state, 600, ["roof", "sloped","3_starting_pots","high_gravity"],0))
        add_rule(world.get_location("High Gravity (2)", player),
                 lambda state: state.has("High Gravity", player) and can_beat_power_level(state, 600, ["roof", "sloped","3_starting_pots","high_gravity"],0))
        add_rule(world.get_location("Squash Showdown! 2 (1)", player),
                 lambda state: state.has("Squash Showdown! 2", player) and can_beat_conveyor(state,["Lily Pad","Squash"],["Nyan Squash","Jalapeno","Ice-shroom","Hypno-shroom"],0.50))
        add_rule(world.get_location("Squash Showdown! 2 (2)", player),
                 lambda state: state.has("Squash Showdown! 2", player) and can_beat_conveyor(state,["Lily Pad","Squash"],["Nyan Squash","Jalapeno","Ice-shroom","Hypno-shroom"],0.50))

        add_rule(world.get_location("Zombies VS Zombies 2 (1)", player),
                 lambda state: state.has("Zombies VS Zombies 2", player))
        add_rule(world.get_location("Zombies VS Zombies 2 (2)", player),
                 lambda state: state.has("Zombies VS Zombies 2", player))
        add_rule(world.get_location("Splash and Clash (1)", player),  # locked and loaded
                 lambda state: state.has("Splash and Clash", player) and state.count("Seed Slot", player)>4)
        add_rule(world.get_location("Splash and Clash (2)", player),
                 lambda state: state.has("Splash and Clash", player) and state.count("Seed Slot", player)>4)

        add_rule(world.get_location("Melon Ninja (1)", player),
                 lambda state: state.has("Melon Ninja", player))
        add_rule(world.get_location("Melon Ninja (2)", player),
                 lambda state: state.has("Melon Ninja", player))
        add_rule(world.get_location("Eclipse (1)", player),
                 lambda state: state.has("Eclipse", player) and can_beat_power_level(state, 500, ["night"],0))
        add_rule(world.get_location("Eclipse (2)", player),
                 lambda state: state.has("Eclipse", player) and can_beat_power_level(state, 500, ["night"],0))

        add_rule(world.get_location("Wall-nut Bowling (1)", player),
                 lambda state: state.has("Wall-nut Bowling", player) and can_beat_conveyor(state,["Wall-nut"],["Ice-shroom","Chomper","Squash","Jalapeno","Cherry Bomb","Doom-shroom","Spikeweed","Hypno-shroom"],0.65))
        add_rule(world.get_location("Wall-nut Bowling (2)", player),
                 lambda state: state.has("Wall-nut Bowling", player) and can_beat_conveyor(state,["Wall-nut"],["Ice-shroom","Chomper","Squash","Jalapeno","Cherry Bomb","Doom-shroom","Spikeweed","Hypno-shroom"],0.65))

        add_rule(world.get_location("Big Trouble Little Zombie (1)", player),
                 lambda state: state.has("Big Trouble Little Zombie", player) and can_beat_conveyor(state,["Puff-shroom","Bamblock"],["Ice-shroom","Saw-me-not","Aloe Aqua","Snow Lotus"],0.75))
        add_rule(world.get_location("Big Trouble Little Zombie (2)", player),
                 lambda state: state.has("Big Trouble Little Zombie", player) and can_beat_conveyor(state,["Puff-shroom","Bamblock"],["Ice-shroom","Saw-me-not","Aloe Aqua","Snow Lotus"],0.75))

        add_rule(world.get_location("True Art is an Explosion 2 (1)", player),
                 lambda state: state.has("True Art is an Explosion 2", player) and can_beat_conveyor(state,[],["Doom-shroom","Blover","Cherry Bomb","Jalapeno","Ice-shroom","Starfruit","Threepeater","Squash"],0.8))
        add_rule(world.get_location("True Art is an Explosion 2 (2)", player),
                 lambda state: state.has("True Art is an Explosion 2", player) and can_beat_conveyor(state,[],["Doom-shroom","Blover","Cherry Bomb","Jalapeno","Ice-shroom","Starfruit","Threepeater","Squash"],0.8))

        add_rule(world.get_location("Graveout (1)", player),
                 lambda state: state.has("Graveout", player))
        add_rule(world.get_location("Graveout (2)", player),
                 lambda state: state.has("Graveout", player))

        add_rule(world.get_location("The Floor is Lava (1)", player),
                 lambda state: can_beat_power_level(state, 800, [],0) and state.has("The Floor is Lava", player))
        add_rule(world.get_location("The Floor is Lava (2)", player),
                 lambda state: can_beat_power_level(state, 800, [],0) and state.has("The Floor is Lava", player))

        add_rule(world.get_location("Beghouled 2: Botany Crush (1)", player),
                 lambda state: state.has("Beghouled 2: Botany Crush", player))
        add_rule(world.get_location("Beghouled 2: Botany Crush (2)", player),
                 lambda state: state.has("Beghouled 2: Botany Crush", player))

        add_rule(world.get_location("Art Challenge: Wall-nut (1)", player),
                 lambda state: state.has("Art Challenge: Wall-nut", player))
        add_rule(world.get_location("Art Challenge: Wall-nut (2)", player),
                 lambda state: state.has("Art Challenge: Wall-nut", player))
        rf.assign_rule("Art Challenge: Wall-nut (1)",
                       "PEA+SUN+CHER+WALL+MINE+CHOM+FUM+SCAR+HYP+SCAR+ICE+DOOM+SQUA+THRE+JAL+SPIK+TORCH+PLANT+CACT+BLOV+STAR+MAGN+CAB+KER+GARL+UMBRE+MEL")#todo this
        rf.assign_rule("Art Challenge: Wall-nut (2)",
                       "PEA+SUN+CHER+WALL+MINE+CHOM+FUM+SCAR+HYP+SCAR+ICE+DOOM+SQUA+THRE+JAL+SPIK+TORCH+PLANT+CACT+BLOV+STAR+MAGN+CAB+KER+GARL+UMBRE+MEL")

        add_rule(world.get_location("Lava Land (1)", player),
                 lambda state: state.has("Lava Land", player)  and can_beat_power_level(state, 800,["water","roof","balloon", "tough_balloon"],0))
        add_rule(world.get_location("Lava Land (2)", player),
                 lambda state: state.has("Lava Land", player)  and can_beat_power_level(state, 800,["water","roof","balloon", "tough_balloon"],0))



        if options.minigame_sanity == 2:
            add_rule(world.get_location("Scaredy's Dream (1)", player),lambda state: state.has("Peashooter",player) and state.has("Scaredy's Dream",player)) #beatable with instas+ chomper
            add_rule(world.get_location("Scaredy's Dream (2)", player),lambda state: state.has("Peashooter",player) and state.has("Scaredy's Dream",player)) #sawmenot, cob cannon,

            add_rule(world.get_location("Pole Vaulting Disco (1)", player),
                     lambda state: can_beat_power_level(state, 400,[],0) and state.has("Pole Vaulting Disco",player))
            add_rule(world.get_location("Pole Vaulting Disco (2)", player),
                     lambda state: can_beat_power_level(state, 400, [],0) and state.has("Pole Vaulting Disco",player))





            add_rule(world.get_location("Z-Day (1)", player),
                 lambda state: can_beat_power_level(state, 300, ["water"],0) and state.has("Z-Day",player))
            add_rule(world.get_location("Z-Day (2)", player),
                 lambda state: can_beat_power_level(state, 300, ["water"],0) and state.has("Z-Day",player))



            add_rule(world.get_location("Columns Like You See 'Em (1)", player),
                 lambda state: can_beat_power_level(state, 1000, ["cherry_newspaper"],0) and state.has("Columns Like You See 'Em",player))
            add_rule(world.get_location("Columns Like You See 'Em (2)", player),
                 lambda state: can_beat_power_level(state, 1000, ["cherry_newspaper"],0) and state.has("Columns Like You See 'Em",player))

            add_rule(world.get_location("Mirrors Like You See 'Em (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"],0) and state.has("Mirrors Like You See 'Em",player))
            add_rule(world.get_location("Mirrors Like You See 'Em (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["cherry_newspaper"],0) and state.has("Mirrors Like You See 'Em",player))

            add_rule(world.get_location("It's Raining Seeds (1)", player),
                     lambda state: state.has("It's Raining Seeds", player) and state.has("Lily Pad",player))
            add_rule(world.get_location("It's Raining Seeds (2)", player),
                     lambda state: state.has("It's Raining Seeds", player) and state.has("Lily Pad",player))


            add_rule(world.get_location("Last Stand (1)", player),
                 lambda state: can_beat_power_level(state, 800, ["water","balloon","tough_balloon"],0) and state.has("Last Stand",player))
            add_rule(world.get_location("Last Stand (2)", player),
                 lambda state: can_beat_power_level(state, 800, ["water","balloon","tough_balloon"],0) and state.has("Last Stand",player))

            add_rule(world.get_location("Air Raid (1)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","balloon","tough_balloon","night"],0) and state.has("Air Raid",player))
            add_rule(world.get_location("Air Raid (2)", player),
                 lambda state: can_beat_power_level(state, 400, ["water","balloon","tough_balloon","night"],0) and state.has("Air Raid",player))

            add_rule(world.get_location("Advanced Challenge: 12-Lane Day (1)", player),
                 lambda state: can_beat_power_level(state, 1000, ["balloon","tough_balloon"],0) and state.has("Advanced Challenge: 12-Lane Day",player))
            add_rule(world.get_location("Advanced Challenge: 12-Lane Day (2)", player),
                 lambda state: can_beat_power_level(state, 1000, ["balloon","tough_balloon"],0) and state.has("Advanced Challenge: 12-Lane Day",player))

            add_rule(world.get_location("Advanced Challenge: 12-Lane Pool (1)", player),
                 lambda state: can_beat_power_level(state, 1000, ["water","large_water","balloon","tough_balloon","cherry_newspaper"],0) and state.has("Advanced Challenge: 12-Lane Pool",player))
            add_rule(world.get_location("Advanced Challenge: 12-Lane Pool (2)", player),
                 lambda state: can_beat_power_level(state, 1000, ["water","large_water","balloon","tough_balloon","cherry_newspaper"],0) and state.has("Advanced Challenge: 12-Lane Pool",player))

            add_rule(world.get_location("True Art is an Explosion! (1)", player),
                     lambda state: can_beat_power_level(state, 200, ["roof", "balloon","tough_balloon","cherry_newspaper","sloped","3_starting_pots"],0) and state.has("True Art is an Explosion!", player))
            add_rule(world.get_location("True Art is an Explosion! (2)", player),
                     lambda state: can_beat_power_level(state, 200, ["roof", "balloon","tough_balloon","cherry_newspaper","sloped","3_starting_pots"],0) and state.has("True Art is an Explosion!", player))



            add_rule(world.get_location("Attack on Gargantuar! (1)", player),
                 lambda state: can_beat_power_level(state, 1000,["roof", "sloped", "3_starting_pots"],0) and state.has("Attack on Gargantuar!", player))
            add_rule(world.get_location("Attack on Gargantuar! (2)", player),
                 lambda state: can_beat_power_level(state, 1000,["roof", "sloped", "3_starting_pots"],0) and state.has("Attack on Gargantuar!", player))

            add_rule(world.get_location("Zum-nut! (1)", player),
                     lambda state: state.has("Zum-nut!", player))
            add_rule(world.get_location("Zum-nut! (2)", player),
                     lambda state: state.has("Zum-nut!", player))

            add_rule(world.get_location("Squash Showdown! (1)", player),
                 lambda state: state.has("Squash Showdown!", player) and can_beat_conveyor(state,["Squash","Peashooter","Ice-shroom","Torchwood"],["Jalapeno","Pumpkin"],0))
            add_rule(world.get_location("Squash Showdown! (2)", player),
                 lambda state: state.has("Squash Showdown!", player) and can_beat_conveyor(state,["Squash","Peashooter","Ice-shroom","Torchwood"],["Jalapeno","Pumpkin"],0))
            # "Squash Showdown!" (squash)(gatling snow)(squashwood)(pumpkin)(spicy squash)

            add_rule(world.get_location("Hypno-nut (1)", player),
                 lambda state: state.has("Hypno-nut", player) and can_beat_conveyor(state,[],["Wall-nut","Squash","Cherry Bomb","Scaredy-shroom","Hypno-shroom","Chomper","Potato Mine","Spikeweed","Threepeater","Fume-shroom","Ice-shroom","Umbrella Leaf","Kernel-pult"],0.68))
            add_rule(world.get_location("Hypno-nut (2)", player),
                 lambda state: state.has("Hypno-nut", player) and can_beat_conveyor(state,[],["Wall-nut","Squash","Cherry Bomb","Scaredy-shroom","Hypno-shroom","Chomper","Potato Mine","Spikeweed","Threepeater","Fume-shroom","Ice-shroom","Umbrella Leaf","Kernel-pult"],0.68))

            # "Hypno-nut" (wallnut)(spike spreader)(icyfumeshroom)(chomper)(scaredy shroom)(squash)(hypnoshroom)(cherry bomb)(potato mine)(butter umbrella)

            add_rule(world.get_location("Dr Zomboss' Revenge (1)", player),
                 lambda state: state.has("Dr Zomboss' Revenge", player))
            add_rule(world.get_location("Dr Zomboss' Revenge (2)", player),
                 lambda state: state.has("Dr Zomboss' Revenge", player))
            rf.assign_rule("Dr Zomboss' Revenge (1)", "POT+KER+CAB+MEL+UMBRE+MARI+GLOV+ICE+JAL+MAL")
            rf.assign_rule("Dr Zomboss' Revenge (2)", "POT+KER+CAB+MEL+UMBRE+MARI+GLOV+ICE+JAL+MAL")

            add_rule(world.get_location("Protect the Gold Magnet (1)", player),
                 lambda state: can_beat_power_level(state, 600,["cherry_newspaper","night"],0) and state.has("Protect the Gold Magnet", player))
            add_rule(world.get_location("Protect the Gold Magnet (2)", player),
                 lambda state: can_beat_power_level(state, 600,["cherry_newspaper","night"],0) and state.has("Protect the Gold Magnet", player))

            add_rule(world.get_location("Compact Planting 2 (1)", player),
                     lambda state: can_beat_power_level(state, 600,["compact_planting"],0) and state.has("Compact Planting 2",player))
            add_rule(world.get_location("Compact Planting 2 (2)", player),
                     lambda state: can_beat_power_level(state, 600, ["compact_planting"],0) and state.has("Compact Planting 2",player)) #has cherry newspaper but doesnt matter

            add_rule(world.get_location("Wall-nut Billiards 2 (1)", player),
                     lambda state: state.has("Wall-nut Billiards 2", player))
            add_rule(world.get_location("Wall-nut Billiards 2 (2)", player),
                     lambda state: state.has("Wall-nut Billiards 2", player))

            add_rule(world.get_location("Wall-nut Billiards 3 (1)", player),
                     lambda state: state.has("Wall-nut Billiards 3", player))
            add_rule(world.get_location("Wall-nut Billiards 3 (2)", player),
                     lambda state: state.has("Wall-nut Billiards 3", player))

            add_rule(world.get_location("Zombie Nimble Zombie Quick (1)", player),
                 lambda state: state.has("Zombie Nimble Zombie Quick", player) and can_beat_power_level(state, 300,["water"],0))
            add_rule(world.get_location("Zombie Nimble Zombie Quick (2)", player),
                 lambda state: state.has("Zombie Nimble Zombie Quick", player) and can_beat_power_level(state, 300,["water"],0))


            #todo

            add_rule(world.get_location("Chomper Snake (1)", player),
                 lambda state: state.has("Chomper Snake", player))
            add_rule(world.get_location("Chomper Snake (2)", player),
                 lambda state: state.has("Chomper Snake", player))

            add_rule(world.get_location("Chinese Chezz (1)", player),
                 lambda state: state.has("Chinese Chezz", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Chinese Chezz (2)", player),
                 lambda state: state.has("Chinese Chezz", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("2048: Pea-volution (1)", player),
                 lambda state: state.has("2048: Pea-volution", player))
            add_rule(world.get_location("2048: Pea-volution (2)", player),
                 lambda state: state.has("2048: Pea-volution", player))

            add_rule(world.get_location("Iceborg Executrix's Revenge (1)", player),
                 lambda state: state.has("Iceborg Executrix's Revenge", player) and can_beat_conveyor(state, ["Bamblock","Firnace","Spruce Sharpshooter","Aloe Aqua","Snow Lotus","Saw-me-not"], [], 0))
            add_rule(world.get_location("Iceborg Executrix's Revenge (2)", player),
                 lambda state: state.has("Iceborg Executrix's Revenge", player) and can_beat_conveyor(state, ["Bamblock","Firnace","Spruce Sharpshooter","Aloe Aqua","Snow Lotus","Saw-me-not"], [], 0))

            add_rule(world.get_location("Capture the Flag (1)", player),
                 lambda state: can_beat_power_level(state, 1000,["flag_capture","night"],0) and state.has("Capture the Flag", player))
            add_rule(world.get_location("Capture the Flag (2)", player),
                 lambda state: can_beat_power_level(state, 1000,["flag_capture","night"],0) and state.has("Capture the Flag", player))





            add_rule(world.get_location("Attack on Gargantuar! 2 (1)", player),
                 lambda state: can_beat_power_level(state, 1000,["roof", "sloped", "3_starting_pots"],0) and state.has("Attack on Gargantuar! 2", player))
            add_rule(world.get_location("Attack on Gargantuar! 2 (2)", player),
                 lambda state: can_beat_power_level(state, 1000,["roof", "sloped", "3_starting_pots"],0) and state.has("Attack on Gargantuar! 2", player))

            add_rule(world.get_location("Graveout 2 (1)", player),
                 lambda state: state.has("Graveout 2", player))
            add_rule(world.get_location("Graveout 2 (2)", player),
                 lambda state: state.has("Graveout 2", player))

            add_rule(world.get_location("I, Zombie (Minigame) (1)", player),
                 lambda state:  state.has("I, Zombie (Minigame)", player))
            add_rule(world.get_location("I, Zombie (Minigame) (2)", player),
                 lambda state: state.has("I, Zombie (Minigame)", player))

            add_rule(world.get_location("Archduke's Revenge (1)", player),
                 lambda state:  state.has("Archduke's Revenge", player) and can_beat_power_level(state, 1000,[],0) and state.has("Wall-nut",player) and state.has("Blover",player))
            add_rule(world.get_location("Archduke's Revenge (2)", player),
                 lambda state: state.has("Archduke's Revenge", player) and can_beat_power_level(state, 1000,[],0) and state.has("Wall-nut",player) and state.has("Blover",player))

            add_rule(world.get_location("Nut-o-mite (1)", player),
                 lambda state:  state.has("Nut-o-mite", player))
            add_rule(world.get_location("Nut-o-mite (2)", player),
                 lambda state: state.has("Nut-o-mite", player))

            add_rule(world.get_location("Nutsweeper (1)", player),
                 lambda state:  state.has("Nutsweeper", player))
            add_rule(world.get_location("Nutsweeper (2)", player),
                 lambda state: state.has("Nutsweeper", player))


    if options.adventure_odyssey or options.goal_type == 2:
            add_rule(world.get_location("Odyssey Adventure: Level 1 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>0)
            add_rule(world.get_location("Odyssey Adventure: Level 1 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>0)

            add_rule(world.get_location("Odyssey Adventure: Level 2 (1)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey"],3) and state.count("Progressive Odyssey Adventure", player)>1)
            add_rule(world.get_location("Odyssey Adventure: Level 2 (2)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey"],3) and state.count("Progressive Odyssey Adventure", player)>1)

            add_rule(world.get_location("Odyssey Adventure: Level 3 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>2)
            add_rule(world.get_location("Odyssey Adventure: Level 3 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>2)

            add_rule(world.get_location("Odyssey Adventure: Level 4 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>3)
            add_rule(world.get_location("Odyssey Adventure: Level 4 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>3)

            add_rule(world.get_location("Odyssey Adventure: Level 5 (1)", player),
                 lambda state: can_beat_power_level(state, 1400,["odyssey"],3) and state.count("Progressive Odyssey Adventure", player)>4)
            add_rule(world.get_location("Odyssey Adventure: Level 5 (2)", player),
                 lambda state: can_beat_power_level(state, 1400,["odyssey"],3) and state.count("Progressive Odyssey Adventure", player)>4)

            add_rule(world.get_location("Odyssey Adventure: Level 6 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>5)
            add_rule(world.get_location("Odyssey Adventure: Level 6 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>5)

            add_rule(world.get_location("Odyssey Adventure: Level 7 (1)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey","water"],3) and state.count("Progressive Odyssey Adventure", player)>6)
            add_rule(world.get_location("Odyssey Adventure: Level 7 (2)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey","water"],3) and  state.count("Progressive Odyssey Adventure", player)>6)

            add_rule(world.get_location("Odyssey Adventure: Level 8 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>7)
            add_rule(world.get_location("Odyssey Adventure: Level 8 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>7)

            add_rule(world.get_location("Odyssey Adventure: Level 9 (1)", player),#fog, zompellins, 3 slots, trident nut is the strongest
                 lambda state: can_beat_power_level(state, 1200,["odyssey","water","balloon","tough_balloon","digger","cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure", player)>8)
            add_rule(world.get_location("Odyssey Adventure: Level 9 (2)", player),
                 lambda state: can_beat_power_level(state, 1200,["odyssey","water","balloon","tough_balloon","digger","cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure", player)>8)

            add_rule(world.get_location("Odyssey Adventure: Level 10 (1)", player),#cherry newspaper, flat roof
                 lambda state: can_beat_power_level(state, 1200,["odyssey","roof","cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure", player)>9)
            add_rule(world.get_location("Odyssey Adventure: Level 10 (2)", player),
                 lambda state: can_beat_power_level(state, 1200,["odyssey","roof","cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure", player)>9)

            add_rule(world.get_location("Odyssey Adventure: Level 11 (1)", player),#gladiantuar, flat roof, require peashooter?
                lambda state: can_beat_power_level(state, 1800, ["odyssey", "roof", "cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure",player) > 10 and state.has("Peashooter",player))
            add_rule(world.get_location("Odyssey Adventure: Level 11 (2)", player),
                 lambda state: can_beat_power_level(state, 1800, ["odyssey", "roof", "cherry_newspaper"],3) and state.count("Progressive Odyssey Adventure",player) > 10 and state.has("Peashooter",player))

            add_rule(world.get_location("Odyssey Adventure: Level 12 (1)", player),#tougher balloon
                 lambda state: can_beat_power_level(state, 1000, ["odyssey", "roof", "balloon","tough_balloon"],3) and state.count("Progressive Odyssey Adventure",player) > 11)
            add_rule(world.get_location("Odyssey Adventure: Level 12 (2)", player),
                 lambda state: can_beat_power_level(state, 1000, ["odyssey", "roof", "balloon","tough_balloon"],3) and state.count("Progressive Odyssey Adventure",player) > 11)

            add_rule(world.get_location("Odyssey Adventure: Level 13 (1)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>12 and state.has("Lily Pad",player))
            add_rule(world.get_location("Odyssey Adventure: Level 13 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>12 and state.has("Lily Pad",player))

            add_rule(world.get_location("Odyssey Adventure: Level 14 (1)", player),#none
                 lambda state: state.count("Progressive Odyssey Adventure", player)>13)
            add_rule(world.get_location("Odyssey Adventure: Level 14 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>13)

            add_rule(world.get_location("Odyssey Adventure: Level 15 (1)", player),#probably 800 power? 3 slots taken
                 lambda state: state.count("Progressive Odyssey Adventure", player)>14 and can_beat_power_level(state, 800, ["odyssey"],3))
            add_rule(world.get_location("Odyssey Adventure: Level 15 (2)", player),
                 lambda state: state.count("Progressive Odyssey Adventure", player)>14 and can_beat_power_level(state, 800, ["odyssey"],3))


    if options.odyssey_minigames>0:
        add_rule(world.get_location("The Gods 1: Trial of Ascension (1)", player),
            lambda state: can_beat_conveyor(state, [], ["Peashooter","Scaredy-shroom","Starfruit","Chomper"], 0.5)and state.has("Plant Gloves", player) and state.has("The Gods 1: Trial of Ascension", player))
        add_rule(world.get_location("The Gods 1: Trial of Ascension (2)", player),
            lambda state: can_beat_conveyor(state, [], ["Peashooter","Scaredy-shroom","Starfruit","Chomper"], 0.5)and state.has("Plant Gloves", player)and state.has("The Gods 1: Trial of Ascension", player))

        add_rule(world.get_location("The Gods 2: Trial of Rebirth (1)", player),
            lambda state: can_beat_conveyor(state, ["Puff-shroom","Cabbage-pult","Kernel-pult","Umbrella Leaf","Wall-nut"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 2: Trial of Rebirth", player))
        add_rule(world.get_location("The Gods 2: Trial of Rebirth (2)", player),
            lambda state: can_beat_conveyor(state, ["Puff-shroom","Cabbage-pult","Kernel-pult","Umbrella Leaf","Wall-nut"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 2: Trial of Rebirth", player))

        add_rule(world.get_location("The Gods 3: Trial of Endurance (1)", player),
            lambda state: can_beat_conveyor(state, ["Threepeater","Pumpkin","Sunflower"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 3: Trial of Endurance", player))
        add_rule(world.get_location("The Gods 3: Trial of Endurance (2)", player),
            lambda state: can_beat_conveyor(state, ["Threepeater","Pumpkin","Sunflower"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 3: Trial of Endurance", player))

        add_rule(world.get_location("The Gods 4: Trial of Valor (1)", player),
            lambda state: can_beat_conveyor(state, ["Peashooter","Scaredy-shroom","Hypno-shroom","Magnet-shroom"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 4: Trial of Valor", player))
        add_rule(world.get_location("The Gods 4: Trial of Valor (2)", player),
            lambda state: can_beat_conveyor(state, ["Peashooter","Scaredy-shroom","Hypno-shroom","Magnet-shroom"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 4: Trial of Valor", player))

        add_rule(world.get_location("Solar-nut Bowling (1)", player),
            lambda state: state.has("Sunflower", player) and state.has("Wall-nut", player) and state.has("Solar-nut Bowling", player))
        add_rule(world.get_location("Solar-nut Bowling (2)", player),
            lambda state: state.has("Sunflower", player) and state.has("Wall-nut", player) and state.has("Solar-nut Bowling", player) )

        add_rule(world.get_location("The Battle Zombies (1)", player),
            lambda state: state.has("Hypno-shroom", player) and state.has("Plant Gloves", player) and state.has("The Battle Zombies", player))
        add_rule(world.get_location("The Battle Zombies (2)", player),
            lambda state: state.has("Hypno-shroom", player) and state.has("Plant Gloves", player) and state.has("The Battle Zombies", player) )

        add_rule(world.get_location("Solitary Spear (1)", player),
            lambda state: state.has("Solitary Spear", player))
        add_rule(world.get_location("Solitary Spear (2)", player),
            lambda state: state.has("Solitary Spear", player) )

        add_rule(world.get_location("The Gods 5: Trial of Radiance (1)", player),
            lambda state: can_beat_conveyor(state, ["Fume-shroom","Cabbage-pult","Lily Pad","Umbrella Leaf"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 5: Trial of Radiance", player))
        add_rule(world.get_location("The Gods 5: Trial of Radiance (2)", player),
            lambda state: can_beat_conveyor(state, ["Fume-shroom","Cabbage-pult","Lily Pad","Umbrella Leaf"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 5: Trial of Radiance", player))

        add_rule(world.get_location("Whack-a-Zombie 2 (1)", player),
            lambda state: state.has("Whack-a-Zombie 2", player) and state.has("Mallet", player))
        add_rule(world.get_location("Whack-a-Zombie 2 (2)", player),
            lambda state: state.has("Whack-a-Zombie 2", player) and state.has("Mallet", player) )

        add_rule(world.get_location("The Gods 7: Trial Resurgence (1)", player),
            lambda state: can_beat_conveyor(state, ["Sunflower","Peashooter","Chomper","Kernel-pult"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 7: Trial Resurgence", player))
        add_rule(world.get_location("The Gods 7: Trial Resurgence (2)", player),
            lambda state: can_beat_conveyor(state, ["Sunflower","Peashooter","Chomper","Kernel-pult"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 7: Trial Resurgence", player))

        add_rule(world.get_location("Barley Battle! (1)", player),
            lambda state: state.has("Barley Battle!", player) and state.has("Plant Gloves", player))
        add_rule(world.get_location("Barley Battle! (2)", player),
            lambda state: state.has("Barley Battle!", player) and state.has("Plant Gloves", player) )

        add_rule(world.get_location("The Gods 6: Trial of Precision (1)", player),
            lambda state: can_beat_conveyor(state, ["Melon-pult","Peashooter","Tangle Kelp","Magnet-shroom"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 6: Trial of Precision", player))
        add_rule(world.get_location("The Gods 6: Trial of Precision (2)", player),
            lambda state: can_beat_conveyor(state, ["Melon-pult","Peashooter","Tangle Kelp","Magnet-shroom"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 6: Trial of Precision", player))

        add_rule(world.get_location("Zombies vs Zombies: Odyssey (1)", player),
            lambda state: state.has("Zombies vs Zombies: Odyssey", player))
        add_rule(world.get_location("Zombies vs Zombies: Odyssey (2)", player),
            lambda state: state.has("Zombies vs Zombies: Odyssey", player))

        add_rule(world.get_location("The Gods 8: Trial of Acclimation (1)", player),
            lambda state: can_beat_conveyor(state, ["Melon-pult","Wall-nut","Spikeweed","Kernel-pult","Cabbage-pult"], [], 0) and state.has("Plant Gloves", player)and state.has("The Gods 8: Trial of Acclimation", player))
        add_rule(world.get_location("The Gods 8: Trial of Acclimation (2)", player),
            lambda state: can_beat_conveyor(state, ["Melon-pult","Wall-nut","Spikeweed","Kernel-pult","Cabbage-pult"], [], 0)and state.has("Plant Gloves", player)and state.has("The Gods 8: Trial of Acclimation", player))

        if options.odyssey_minigames == 2:

            add_rule(world.get_location("Odyssey: Last Stand (1)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey","plant_storage","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Last Stand", player))
            add_rule(world.get_location("Odyssey: Last Stand (2)", player),
                 lambda state: can_beat_power_level(state, 1600,["odyssey","plant_storage","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Last Stand", player))





            add_rule(world.get_location("Ten-Flag The Gods 1 (1)", player),
                lambda state: can_beat_conveyor(state, ["Peashooter","Scaredy-shroom","Starfruit","Chomper"], [], 0)and state.has("Plant Gloves", player) and state.has("Ten-Flag The Gods 1", player))
            add_rule(world.get_location("Ten-Flag The Gods 1 (2)", player),
                lambda state: can_beat_conveyor(state, ["Peashooter","Scaredy-shroom","Starfruit","Chomper"], [], 0)and state.has("Plant Gloves", player)and state.has("Ten-Flag The Gods 1", player))

            add_rule(world.get_location("Ten-Flag The Gods 2 (1)", player),
                lambda state: can_beat_conveyor(state, ["Puff-shroom","Cabbage-pult","Kernel-pult","Umbrella Leaf","Wall-nut"], [], 0)and state.has("Plant Gloves", player) and state.has("Ten-Flag The Gods 2", player))
            add_rule(world.get_location("Ten-Flag The Gods 2 (2)", player),
                lambda state: can_beat_conveyor(state, ["Puff-shroom","Cabbage-pult","Kernel-pult","Umbrella Leaf","Wall-nut"], [], 0)and state.has("Plant Gloves", player)and state.has("Ten-Flag The Gods 2", player))

            add_rule(world.get_location("Ten-Flag The Gods 3 (1)", player),
                lambda state: can_beat_conveyor(state, ["Threepeater","Pumpkin","Sunflower","Hypno-shroom"], [], 0)and state.has("Plant Gloves", player) and state.has("Ten-Flag The Gods 3", player))
            add_rule(world.get_location("Ten-Flag The Gods 3 (2)", player),
                lambda state: can_beat_conveyor(state, ["Threepeater","Pumpkin","Sunflower","Hypno-shroom"], [], 0)and state.has("Plant Gloves", player)and state.has("Ten-Flag The Gods 3", player))

            add_rule(world.get_location("Odyssey: Rush Mode (1)", player),
                lambda state: can_beat_power_level(state, 1600,["odyssey","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Rush Mode", player))
            add_rule(world.get_location("Odyssey: Rush Mode (2)", player),
                lambda state: can_beat_power_level(state, 1600,["odyssey","plant_storage","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Rush Mode", player))

            add_rule(world.get_location("Gacha Battle (1)", player),
                lambda state: state.has("Gacha Battle", player))
            add_rule(world.get_location("Gacha Battle (2)", player),
                lambda state: state.has("Gacha Battle", player))

            add_rule(world.get_location("Odyssey: Randomized (1)", player),
                lambda state: can_beat_power_level(state, 1600,["odyssey","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Randomized", player))
            add_rule(world.get_location("Odyssey: Randomized (2)", player),
                lambda state: can_beat_power_level(state, 1600,["odyssey","odyssey_upgrades","hard_cherry","harder_balloon"],0) and state.has("Odyssey: Randomized", player))

            add_rule(world.get_location("Ten-Flag Odyssey Gacha (1)", player),
                lambda state: state.has("Ten-Flag Odyssey Gacha", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))
            add_rule(world.get_location("Ten-Flag Odyssey Gacha (2)", player),
                lambda state: state.has("Ten-Flag Odyssey Gacha", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))

            add_rule(world.get_location("Odyssey Gacha: 12-Lane (1)", player),
                lambda state: state.has("Odyssey Gacha: 12-Lane", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))
            add_rule(world.get_location("Odyssey Gacha: 12-Lane (2)", player),
                lambda state: state.has("Odyssey Gacha: 12-Lane", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))

            add_rule(world.get_location("Ten-Flag Equivalent Exchange 2 (1)", player),
                lambda state: state.has("Ten-Flag Equivalent Exchange 2", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Ten-Flag Equivalent Exchange 2 (2)", player),
                lambda state: state.has("Ten-Flag Equivalent Exchange 2", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Ten-Flag Super Conveyor Belt (1)", player),
                lambda state: state.has("Ten-Flag Super Conveyor Belt", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Ten-Flag Super Conveyor Belt (2)", player),
                lambda state: state.has("Ten-Flag Super Conveyor Belt", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Imitater (1)", player),
                lambda state: state.has("Odyssey Gacha: Imitater", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Imitater (2)", player),
                lambda state: state.has("Odyssey Gacha: Imitater", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Unleashed (1)", player),
                lambda state: state.has("Odyssey Gacha: Unleashed", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Unleashed (2)", player),
                lambda state: state.has("Odyssey Gacha: Unleashed", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Barley Battle 2! Fission (1)", player),
                lambda state: state.has("Barley Battle 2! Fission", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Barley Battle 2! Fission (2)", player),
                lambda state: state.has("Barley Battle 2! Fission", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Fusion Mode (1)", player),
                lambda state: can_beat_power_level(state, 1400,["odyssey","hard_cherry","harder_balloon"],0) and state.has("Odyssey Gacha: Fusion Mode", player))
            add_rule(world.get_location("Odyssey Gacha: Fusion Mode (2)", player),
                lambda state: can_beat_power_level(state, 1400,["odyssey","hard_cherry","harder_balloon"],0) and state.has("Odyssey Gacha: Fusion Mode", player))

            add_rule(world.get_location("Odyssey Gacha: Fusion Mode 2 (1)", player),
                lambda state: can_beat_power_level(state, 1400,["odyssey","hard_cherry","harder_balloon"],0) and state.has("Odyssey Gacha: Fusion Mode 2", player))
            add_rule(world.get_location("Odyssey Gacha: Fusion Mode 2 (2)", player),
                lambda state: can_beat_power_level(state, 1400,["odyssey","hard_cherry","harder_balloon"],0) and state.has("Odyssey Gacha: Fusion Mode 2", player))

            add_rule(world.get_location("Odyssey Gacha: Upgrade (1)", player),
                lambda state: state.has("Odyssey Gacha: Upgrade", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Upgrade (2)", player),
                lambda state: state.has("Odyssey Gacha: Upgrade", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Blessings and Curses (1)", player),
                lambda state: state.has("Odyssey Gacha: Blessings and Curses", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))
            add_rule(world.get_location("Odyssey Gacha: Blessings and Curses (2)", player),
                lambda state: state.has("Odyssey Gacha: Blessings and Curses", player) and state.has("Plant Gloves", player) and state.has("Plant Giftbox", player) and state.has("Zombie Giftbox", player))

            add_rule(world.get_location("Odyssey Gacha: Gashapon (1)", player),
                lambda state: state.has("Odyssey Gacha: Gashapon", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Gashapon (2)", player),
                lambda state: state.has("Odyssey Gacha: Gashapon", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Treasure Hunt (1)", player),
                lambda state: state.has("Odyssey Gacha: Treasure Hunt", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Treasure Hunt (2)", player),
                lambda state: state.has("Odyssey Gacha: Treasure Hunt", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Diamond Imitater (1)", player),
                lambda state: state.has("Odyssey Gacha: Diamond Imitater", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Diamond Imitater (2)", player),
                lambda state: state.has("Odyssey Gacha: Diamond Imitater", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Odyssey Gacha: Rerolled (1)", player),#this is a more reasonable conveyor belt level
                lambda state: state.has("Odyssey Gacha: Rerolled", player) and can_beat_conveyor(state, [], ["Peashooter","Cherry Bomb","Wall-nut","Potato Mine","Chomper","Puff-shroom","Fume-shroom","Hypno-shroom","Scaredy-shroom","Ice-shroom","Doom-shroom","Squash","Threepeater","Jalapeno","Spikeweed","Torchwood","Plantern","Cactus","Blover","Starfruit","Pumpkin","Magnet-shroom","Cabbage-pult","Kernel-pult","Garlic","Umbrella Leaf","Marigold","Melon-pult"], 0.7) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Odyssey Gacha: Rerolled (2)", player),
                lambda state: state.has("Odyssey Gacha: Rerolled", player) and can_beat_conveyor(state, [], ["Peashooter","Cherry Bomb","Wall-nut","Potato Mine","Chomper","Puff-shroom","Fume-shroom","Hypno-shroom","Scaredy-shroom","Ice-shroom","Doom-shroom","Squash","Threepeater","Jalapeno","Spikeweed","Torchwood","Plantern","Cactus","Blover","Starfruit","Pumpkin","Magnet-shroom","Cabbage-pult","Kernel-pult","Garlic","Umbrella Leaf","Marigold","Melon-pult"], 0.7) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Purgatory Gacha: Chibi vs. Goliath (1)", player),
                lambda state: state.has("Purgatory Gacha: Chibi vs. Goliath", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Purgatory Gacha: Chibi vs. Goliath (2)", player),
                lambda state: state.has("Purgatory Gacha: Chibi vs. Goliath", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Fusion Gacha: Chaos (1)", player),
                lambda state: state.has("Fusion Gacha: Chaos", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Fusion Gacha: Chaos (2)", player),
                lambda state: state.has("Fusion Gacha: Chaos", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Advanced Gacha: Chaos (1)", player),
                lambda state: state.has("Advanced Gacha: Chaos", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Advanced Gacha: Chaos (2)", player),
                lambda state: state.has("Advanced Gacha: Chaos", player) and state.has("Plant Gloves", player))

            add_rule(world.get_location("Purgatory Gacha: Chaos (1)", player),
                lambda state: state.has("Purgatory Gacha: Chaos", player) and state.has("Plant Gloves", player))
            add_rule(world.get_location("Purgatory Gacha: Chaos (2)", player),
                lambda state: state.has("Purgatory Gacha: Chaos", player) and state.has("Plant Gloves", player))





    #"Capture the Flag"


    if options.goal_type == 0:
        #rf.assign_rule("Dr. Zomboss' Revenge", "POT+KER+CAB+MEL+UMBRE+MARI+GLOV+ICE+JAL+MAL")
        world.completion_condition[player] = lambda state: state.has("Roof Access", player) and state.has("Flower Pot",player) and state.has("Kernel-pult", player) and state.has("Cabbage-pult", player) and state.has("Melon-pult",player) and state.has("Marigold",player) and state.has("Plant Gloves", player) and state.has("Mallet", player) and state.has("Jalapeno", player) and state.has("Ice-shroom", player)

    if options.goal_type == 1:
        if options.adventure_extra == 2:
            world.completion_condition[player] = lambda state: state.can_reach("Day: Level 9 (1)", "Location", player) and state.can_reach("Night: Level 18 (1)", "Location", player) and state.can_reach("Pool: Level 27 (1)", "Location", player) and state.can_reach("Fog: Level 36 (1)", "Location", player) and state.can_reach("Roof: Level 45 (1)", "Location", player) and state.can_reach("Snow: Level 9 (1)", "Location", player)
        else:
            world.completion_condition[player] = lambda state: state.can_reach("Day: Level 9 (1)", "Location", player) and state.can_reach("Night: Level 18 (1)", "Location", player) and state.can_reach("Pool: Level 27 (1)", "Location", player) and state.can_reach("Fog: Level 36 (1)", "Location", player) and state.can_reach("Roof: Level 45 (1)", "Location",player)
    if options.goal_type == 2:
        world.completion_condition[player] = lambda state: state.can_reach_location("Odyssey Adventure: Level 15 (1)",player)

    if options.goal_type == 3:
        world.completion_condition[player] = lambda state: state.count("Trophy",player) >= int(options.trophy_number * (options.trophy_percentage/100))
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
        "SCAR": "Scaredy-shroom",
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
        "SEA": "Sea-shroom",
        "PLANT": "Plantern",
        "CACT": "Cactus",
        "BLOV": "Blover",
        "STAR": "Starfruit",
        "PUMPK": "Pumpkin",
        "MAGN": "Magnet-shroom",
        "CAT": "Cattail",
        "CAB": "Cabbage-pult",
        "POT": "Flower Pot",
        "KER": "Kernel-pult",
        "GARL":"Garlic",
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
        "NEKO": "Nyan Squash",
        "GLOV": "Plant Gloves",
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

