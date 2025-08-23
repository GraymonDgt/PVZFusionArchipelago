import typing
from enum import Enum

from BaseClasses import MultiWorld, Region, Entrance, Location
from .Options import PVZFOptions
from .Locations import PVZFLocation, location_table# ,GFZ_table,THZ_table,DSZ_table,CEZ_table,ACZ_table,
    #RVZ_table,ERZ_table,BCZ_table

class SRB2Zones(int, Enum):
    GREENFLOWER = 1
    TECHNO_HILL = 2
    DEEP_SEA = 3
    CASTLE_EGGMAN = 4
    ARID_CANYON = 5
    RED_VOLCANO = 6
    EGG_ROCK = 7
    BLACK_CORE = 8#scared to remove this



class PVZFRegion(Region):
    subregions: typing.List[Region] = []


# sm64paintings is a dict of entrances, format LEVEL | AREA


def create_regions(world: MultiWorld, options: PVZFOptions, player: int):
    regMM = Region("Menu", player, world, "Level Select")
    #create_default_locs(regMM, locSS_table)#TODO this might break something
    world.regions.append(regMM)


    regDay = create_region("Day", player, world)
    create_locs(regDay, "Day: Level 1 (1)","Day: Level 1 (2)",
                "Day: Level 2 (1)","Day: Level 2 (2)",
                "Day: Level 3 (1)","Day: Level 3 (2)",
                "Day: Level 4 (1)","Day: Level 4 (2)",
                "Day: Level 5 (1)","Day: Level 5 (2)",
                "Day: Level 6 (1)","Day: Level 6 (2)",
                "Day: Level 7 (1)","Day: Level 7 (2)",
                "Day: Level 8 (1)","Day: Level 8 (2)",
                "Day: Level 9 (1)","Day: Level 9 (2)")

    regNight = create_region("Night", player, world)
    create_locs(regNight, "Night: Level 10 (1)","Night: Level 10 (2)",
                "Night: Level 11 (1)", "Night: Level 11 (2)",
                "Night: Level 12 (1)", "Night: Level 12 (2)",
                "Night: Level 13 (1)", "Night: Level 13 (2)",
                "Night: Level 14 (1)", "Night: Level 14 (2)",
                "Night: Level 15 (1)", "Night: Level 15 (2)",
                "Night: Level 16 (1)", "Night: Level 16 (2)",
                "Night: Level 17 (1)", "Night: Level 17 (2)",
                "Night: Level 18 (1)", "Night: Level 18 (2)",
                )
    regPool = create_region("Pool", player, world)
    create_locs(regPool,"Pool: Level 19 (1)","Pool: Level 19 (2)",
                "Pool: Level 20 (1)", "Pool: Level 20 (2)",
                "Pool: Level 21 (1)", "Pool: Level 21 (2)",
                "Pool: Level 22 (1)", "Pool: Level 22 (2)",
                "Pool: Level 23 (1)", "Pool: Level 23 (2)",
                "Pool: Level 24 (1)", "Pool: Level 24 (2)",
                "Pool: Level 25 (1)", "Pool: Level 25 (2)",
                "Pool: Level 26 (1)", "Pool: Level 26 (2)",
                "Pool: Level 27 (1)", "Pool: Level 27 (2)",
            )

    regFog = create_region("Fog", player, world)
    create_locs(regFog,"Fog: Level 28 (1)","Fog: Level 28 (2)",
                "Fog: Level 29 (1)","Fog: Level 29 (2)",
                "Fog: Level 30 (1)","Fog: Level 30 (2)",
                "Fog: Level 31 (1)","Fog: Level 31 (2)",
                "Fog: Level 32 (1)","Fog: Level 32 (2)",
                "Fog: Level 33 (1)","Fog: Level 33 (2)",
                "Fog: Level 34 (1)","Fog: Level 34 (2)",
                "Fog: Level 35 (1)","Fog: Level 35 (2)",
                "Fog: Level 36 (1)","Fog: Level 36 (2)",)

    regRoof = create_region("Roof", player, world)
    create_locs(regRoof, "Roof: Level 37 (1)","Roof: Level 37 (2)",
                "Roof: Level 38 (1)", "Roof: Level 38 (2)",
                "Roof: Level 39 (1)", "Roof: Level 39 (2)",
                "Roof: Level 40 (1)", "Roof: Level 40 (2)",
                "Roof: Level 41 (1)", "Roof: Level 41 (2)",
                "Roof: Level 42 (1)", "Roof: Level 42 (2)",
                "Roof: Level 43 (1)", "Roof: Level 43 (2)",
                "Roof: Level 44 (1)", "Roof: Level 44 (2)",
                "Roof: Level 45 (1)", "Roof: Level 45 (2)",

    )
    if options.goal_type == 0 and options.minigame_sanity == 0:
        create_locs(regRoof,"Dr. Zomboss' Revenge")


    regSnow = create_region("Snow", player, world)
    create_locs(regSnow,
    "Snow: Level 1 (1)","Snow: Level 1 (2)",
    "Snow: Level 2 (1)", "Snow: Level 2 (2)",
    "Snow: Level 3 (1)", "Snow: Level 3 (2)",
    "Snow: Level 4 (1)", "Snow: Level 4 (2)",
    "Snow: Level 5 (1)", "Snow: Level 5 (2)",
    "Snow: Level 6 (1)", "Snow: Level 6 (2)",
    "Snow: Level 7 (1)", "Snow: Level 7 (2)",
    "Snow: Level 8 (1)", "Snow: Level 8 (2)",
    "Snow: Level 9 (1)", "Snow: Level 9 (2)",
                )
    if options.challenge_sanity:
        regChal = create_region("Fusion Challenges", player, world)
        create_locs(regChal,
            "Fusion Challenge: Explod-o-shooter (1)",
            "Fusion Challenge: Chompzilla (1)",
            "Fusion Challenge: Charm-shroom (1)",
            "Fusion Challenge: Doomspike-shroom (1)",
            "Fusion Challenge: Infernowood (1)",
            "Fusion Challenge: Krakerberus (1)",
            "Fusion Challenge: Stardrop (1)",
            "Fusion Challenge: Bloverthorn Pumpkin (1)",
            "Fusion Challenge: Salad-pult (1)",
            "Fusion Challenge: Alchemist Umbrella (1)",
            "Fusion Challenge: Spruce Supershooter (1)",

            "Fusion Challenge: Explod-o-shooter (2)",
            "Fusion Challenge: Chompzilla (2)",
            "Fusion Challenge: Charm-shroom (2)",
            "Fusion Challenge: Doomspike-shroom (2)",
            "Fusion Challenge: Infernowood (2)",
            "Fusion Challenge: Krakerberus (2)",
            "Fusion Challenge: Stardrop (2)",
            "Fusion Challenge: Bloverthorn Pumpkin (2)",
            "Fusion Challenge: Salad-pult (2)",
            "Fusion Challenge: Alchemist Umbrella (2)",
            "Fusion Challenge: Spruce Supershooter (2)"
)
    if options.showcase_sanity:
        regShow = create_region("Fusion Showcase", player, world)
        create_locs(regShow,
            "Fusion Showcase: Titan Pea Turret (1)",
            "Fusion Showcase: Explod-o-tato Mine (1)",
            "Fusion Showcase: Pumpkin Bunker (1)",
            "Fusion Showcase: Nugget-shroom (1)",
            "Fusion Showcase: Spuddy-shroom (1)",
            "Fusion Showcase: Chomper Maw (1)",
            "Fusion Showcase: Foul-shroom (1)",
            "Fusion Showcase: Mind-blover (1)",
            "Fusion Showcase: Boomwood (1)",
            "Fusion Showcase: Bamboom (1)",
            "Fusion Showcase: Spike-nut (1)",
            "Fusion Showcase: Leviathan-shroom (1)",

            "Fusion Showcase: Titan Pea Turret (2)",
            "Fusion Showcase: Explod-o-tato Mine (2)",
            "Fusion Showcase: Pumpkin Bunker (2)",
            "Fusion Showcase: Nugget-shroom (2)",
            "Fusion Showcase: Spuddy-shroom (2)",
            "Fusion Showcase: Chomper Maw (2)",
            "Fusion Showcase: Foul-shroom (2)",
            "Fusion Showcase: Mind-blover (2)",
            "Fusion Showcase: Boomwood (2)",
            "Fusion Showcase: Bamboom (2)",
            "Fusion Showcase: Spike-nut (2)",
            "Fusion Showcase: Leviathan-shroom (2)")





    #if options.time_emblems:
    #    create_locs(regGFZ, "Greenflower (Act 1) Time Emblem","Greenflower (Act 2) Time Emblem","Greenflower (Act 3) Time Emblem")


def connect_regions(world: MultiWorld, player: int, source: str, target: str, rule=None) -> Entrance:
    sourceRegion = world.get_region(source, player)
    targetRegion = world.get_region(target, player)
    return sourceRegion.connect(targetRegion, rule=rule)


def create_region(name: str, player: int, world: MultiWorld) -> PVZFRegion:
    region = PVZFRegion(name, player, world)
    world.regions.append(region)
    return region


def create_subregion(source_region: Region, name: str, *locs: str) -> PVZFRegion:
    region = PVZFRegion(name, source_region.player, source_region.multiworld)
    connection = Entrance(source_region.player, name, source_region)
    source_region.exits.append(connection)
    connection.connect(region)
    source_region.multiworld.regions.append(region)
    create_locs(region, *locs)
    return region


def set_subregion_access_rule(world, player, region_name: str, rule):
    world.get_entrance(world, player, region_name).access_rule = rule


def create_default_locs(reg: Region, default_locs: dict):
    create_locs(reg, *default_locs.keys())


def create_locs(reg: Region, *locs: str):
    reg.locations += [PVZFLocation(reg.player, loc_name, location_table[loc_name], reg) for loc_name in locs]
