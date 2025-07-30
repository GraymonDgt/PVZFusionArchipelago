from typing import Callable, Union, Dict, Set

from BaseClasses import MultiWorld
from ..generic.Rules import add_rule, set_rule
from .Locations import location_table
from .Options import PVZFOptions
from .Regions import connect_regions, SRB2Zones
#from .Items import character_item_data_table
#from .Items import tools_item_data_table

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


    rf = RuleFactory(world, options, player, move_rando_bitvec)

    connect_regions(world, player, "Menu", "Day", lambda state: state.has("Day Access", player))
    connect_regions(world, player, "Menu", "Night", lambda state: state.has("Night Access", player))
    connect_regions(world, player, "Menu", "Pool", lambda state: state.has("Pool Access", player))
    connect_regions(world, player, "Menu", "Fog", lambda state: state.has("Fog Access", player))
    connect_regions(world, player, "Menu", "Roof", lambda state: state.has("Roof Access", player))
    connect_regions(world, player, "Menu", "Snow", lambda state: state.has("Snow Access", player))
    #connect_regions(world, player, "Menu", "Fusion Challenges", lambda state: state.has("Fusion Challenges", player))


    #rf.assign_rule("Level 6 (Day)", "SUN") #diamond giftbox
    rf.assign_rule("Day: Level 7 (1)", "CHER & WALL | CHER & PUMPK | CHER & UMBRE | CHER & PEA & GLOV")  # cherry newspaper
    rf.assign_rule("Day: Level 9 (1)", "CHER & WALL | CHER & PUMPK | CHER & UMBRE | CHER & PEA & GLOV")  # cherry newspaper

    rf.assign_rule("Day: Level 7 (2)", "CHER & WALL | CHER & PUMPK | CHER & UMBRE | CHER & PEA & GLOV")  # cherry newspaper
    rf.assign_rule("Day: Level 9 (2)", "CHER & WALL | CHER & PUMPK | CHER & UMBRE | CHER & PEA & GLOV")  # cherry newspaper

    #rf.assign_rule("Level 10 (Night)","SUN | PUFF") # might not be possible without mowers
    #rf.assign_rule("Level 11 (Night)","SUN | PUFF") # not possible without movers

    #rf.assign_rule("Level 12 (Night)", "SUN")
    #rf.assign_rule("Level 13 (Night)", "SUN")
    #rf.assign_rule("Level 14 (Night)", "SUN")
    #rf.assign_rule("Level 15 (Night)", "SUN") 
    #rf.assign_rule("Level 16 (Night)", "SUN")
    #rf.assign_rule("Level 17 (Night)", "SUN") 
    #rf.assign_rule("Level 18 (Night)", "SUN")
#cattail girl carries without sunflowers btw
    rf.assign_rule("Pool: Level 19 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 20 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 21 (1)", "LILY | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT") #maybe kelp+threepeater recharges fast enough?
    rf.assign_rule("Pool: Level 22 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#submarine
    rf.assign_rule("Pool: Level 23 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#submarine
    rf.assign_rule("Pool: Level 24 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#submarine
    rf.assign_rule("Pool: Level 25 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#submarine
    rf.assign_rule("Pool: Level 26 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#submarine
    rf.assign_rule("Pool: Level 27 (1)", "LILY+THRE+SQUA+SPIK+TORCH+WALL+JAL")#kelp also here but its not necessary

    rf.assign_rule("Pool: Level 19 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 20 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 21 (2)", "LILY | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 22 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 23 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 24 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 25 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 26 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
    rf.assign_rule("Pool: Level 27 (2)", "LILY+THRE+SQUA+SPIK+TORCH+WALL+JAL")


    rf.assign_rule("Fog: Level 28 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT") #mowers
    rf.assign_rule("Fog: Level 29 (1)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")

    rf.assign_rule("Fog: Level 30 (1)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")#maybe cob cannon #probably possible with just blover & Threepeater
    rf.assign_rule("Fog: Level 31 (1)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")  # maybe cob cannon
    rf.assign_rule("Fog: Level 32 (1)", "LILY & CACT | LILY & CAT | LILY & FERT")  # maybe allow bombs?
    rf.assign_rule("Fog: Level 33 (1)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")  # digger
    rf.assign_rule("Fog: Level 34 (1)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")  # digger
    rf.assign_rule("Fog: Level 35 (1)", "LILY & CACT | LILY & CAT | LILY & FERT")  # maybe allow bombs?
    rf.assign_rule("Fog: Level 36 (1)", "LILY+CACT+STAR+PLANT+MAGN+PUMPK")  # maybe allow bombs?


    rf.assign_rule("Fog: Level 28 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#dup of above
    rf.assign_rule("Fog: Level 29 (2)", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")

    rf.assign_rule("Fog: Level 30 (2)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 31 (2)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 32 (2)", "LILY & CACT | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 33 (2)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 34 (2)", "LILY & CACT | LILY & BLOV | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 35 (2)", "LILY & CACT | LILY & CAT | LILY & FERT")
    rf.assign_rule("Fog: Level 36 (2)", "LILY+CACT+STAR+PLANT+MAGN+PUMPK")



#
#    #fog 36 needs cactus or starfruit
    rf.assign_rule("Roof: Level 37 (1)", "POT | CAB | KER | FUM | MEL & WALL | MEL & GLOV | MEL & MINE | MEL & SQUA") #melon + glove or melon + wallnut
    rf.assign_rule("Roof: Level 38 (1)", "POT")
    rf.assign_rule("Roof: Level 39 (1)", "POT")
    rf.assign_rule("Roof: Level 40 (1)", "POT")
    rf.assign_rule("Roof: Level 41 (1)", "POT")#needs a giga-nut counter
    rf.assign_rule("Roof: Level 42 (1)", "POT")#catapult + melon pogo
    rf.assign_rule("Roof: Level 43 (1)", "POT")
    rf.assign_rule("Roof: Level 44 (1)", "POT")#explodopults
    rf.assign_rule("Roof: Level 45 (1)", "POT+MEL+KER+CAB+UMBRE+ICE+JAL")

    rf.assign_rule("Roof: Level 37 (2)", "POT | CAB | KER | FUM | MEL & WALL | MEL & GLOV | MEL & MINE | MEL & SQUA")#probably should use regions to make this less error prone
    rf.assign_rule("Roof: Level 38 (2)", "POT")
    rf.assign_rule("Roof: Level 39 (2)", "POT")
    rf.assign_rule("Roof: Level 40 (2)", "POT")
    rf.assign_rule("Roof: Level 41 (2)", "POT")
    rf.assign_rule("Roof: Level 42 (2)", "POT")
    rf.assign_rule("Roof: Level 43 (2)", "POT")
    rf.assign_rule("Roof: Level 44 (2)", "POT")
    rf.assign_rule("Roof: Level 45 (2)", "POT+MEL+KER+CAB+UMBRE+ICE+JAL")

    rf.assign_rule("Snow: Level 1 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 2 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 3 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 4 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 5 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 6 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 7 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 8 (1)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 9 (1)", "FIR+SPRUC+SAW+ALOE+LOTUS+BAMB")



    rf.assign_rule("Snow: Level 1 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 2 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 3 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 4 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 5 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 6 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 7 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 8 (2)", "FIR | CHER | DOOM | ICE | JAL")
    rf.assign_rule("Snow: Level 9 (1)", "FIR+SPRUC+SAW+ALOE+LOTUS+BAMB")



#fume shroom and melon pult have jalapeno fusions
#peashooter fume scaredy melon have ice shroom fusions
#spruce supershooter?



#
##snow levels require explosives or firnace btw
#
#
#
#    rf.assign_rule("Fusion Challenge: Explod-o-shooter","CHER & PEA | CHER & WALL | CHER & PUMPK | CHER & UMBRE")
#    #chompzilla requires high dps
#    #most challenges do
#    rf.assign_rule("Fusion Challenge: Krakerberus", "LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")
#    rf.assign_rule("Fusion Challenge: Bloverthorn Pumpkin","LILY | THRE | STAR | CATGI | KER & COB | KER & FERT | FUM & GLOOM | FUM & FERT | SEA & CACT")#temp logic
#    rf.assign_rule("Fusion Challenge: Salad-pult","POT")  # temp logic, melonpogos, explodopults
#    rf.assign_rule("Fusion Challenge: Alchemist Umbrella", "POT")  # temp logic, melonpogos, explodopults, giganuts
#    rf.assign_rule("Fusion Challenge: Spruce Supershooter", "CHER & PEA | CHER & WALL | CHER & PUMPK | CHER & UMBRE")#ice zombies
#    rf.assign_rule("Dr. Zomboss' Revenge", "POT+KER+CAB+MEL+UMBRE+MARI")
#    # rf.assign_rule("Fusion Challenge: Stardrop","LILY+(CACT|CAT|FERT)")  this can almost clear itself with glove+mallet lmao
#    #if options.completion_type == 0:
    world.completion_condition[player] = lambda state: state.has("Roof Access", player) and state.has("Flower Pot", player) and state.has("Kernel-pult", player) and state.has("Cabbage-pult", player) and state.has("Melon-pult", player) and state.has("Marigold", player) and state.has("Plant Gloves", player) and state.has("Mallet", player)and state.has("Jalapeno", player)and state.has("Ice-shroom", player)
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
        "PUFF": "Puff-shroom",
        "FUM": "Fume-shroom",
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
        "CACT":"Cactus",
        "BLOV": "Blover",
        "STAR": "Starfruit",
        "PUMPK": "Pumpkin",
        "CAT": "Cattail",
        "CAB": "Cabbage-pult",
        "POT": "Flower Pot",
        "KER": "Kernel-pult",
        "UMBRE": "Umbrella Leaf",
        "MARI": "Marigold",
        "MEL": "Melon-pult",
        "COB": "Cob Cannon",
        "FIR": "Firnace",
        "SRPUC": "Spruce Sharpshooter",
        "SAW": "Saw-me-not",
        "LOTUS": "Snow Lotus",
        "ALOE": "Aloe Aqua",
        "BAMB": "Bamblock",
        "CATGI": "Cattail Girl",
        "GLOV": "PLant Gloves",
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

