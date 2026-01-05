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


    if options.adventure_extra == 2:
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
            "Fusion Challenge: Jicamagic (1)",


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
            "Fusion Challenge: Spruce Supershooter (2)",
            "Fusion Challenge: Jicamagic (2)"
        )

    #if options.showcase_sanity:
    #    regShow = create_region("Fusion Showcase", player, world)
    #    create_locs(regShow,
    #        "Fusion Showcase: Explod-o-tato Mine (1)",
    #        "Fusion Showcase: Pumpkin Bunker (1)",
    #        "Fusion Showcase: Nugget-shroom (1)",
    #        "Fusion Showcase: Spuddy-shroom (1)",
    #        "Fusion Showcase: Chomper Maw (1)",
    #        "Fusion Showcase: Foul-shroom (1)",
    #        "Fusion Showcase: Mind-blover (1)",
    #        "Fusion Showcase: Boomwood (1)",
    #        "Fusion Showcase: Bamboom (1)",
    #        "Fusion Showcase: Spike-nut (1)",
    #        "Fusion Showcase: Leviathan-shroom (1)",
#
    #        "Fusion Showcase: Explod-o-tato Mine (2)",
    #        "Fusion Showcase: Pumpkin Bunker (2)",
    #        "Fusion Showcase: Nugget-shroom (2)",
    #        "Fusion Showcase: Spuddy-shroom (2)",
    #        "Fusion Showcase: Chomper Maw (2)",
    #        "Fusion Showcase: Foul-shroom (2)",
    #        "Fusion Showcase: Mind-blover (2)",
    #        "Fusion Showcase: Boomwood (2)",
    #        "Fusion Showcase: Bamboom (2)",
    #        "Fusion Showcase: Spike-nut (2)",
    #        "Fusion Showcase: Leviathan-shroom (2)")

    if options.minigame_sanity!=0:
        regMini = create_region("Minigames", player, world)
        create_locs(regMini,"Compact Planting (1)",
                    "Newspaper War (1)",
                    "Matryoshka (1)",
                    "Pogo Party! (1)",
                    "Bungee Blitz (1)",
                    "Beghouled (1)",
                    "Seeing Stars (1)",
                    "Wall-nut Billiards (1)",
                    "Whack a Zombie (1)",
                    "High Gravity (1)",
                    "Squash Showdown! 2 (1)",
                    "Zombies VS Zombies 2 (1)",
                    "Splash and Clash (1)",
                    "Melon Ninja (1)",
                    "Eclipse (1)",
                    "Wall-nut Bowling (1)",
                    "Big Trouble Little Zombie (1)",
                    "True Art is an Explosion 2 (1)",
                    "Graveout (1)",
                    "The Floor is Lava (1)",
                    "Art Challenge: Wall-nut (1)",
                    "Beghouled 2: Botany Crush (1)",

                    "Compact Planting (2)",
                    "Newspaper War (2)",
                    "Matryoshka (2)",
                    "Pogo Party! (2)",
                    "Bungee Blitz (2)",
                    "Beghouled (2)",
                    "Seeing Stars (2)",
                    "Wall-nut Billiards (2)",
                    "Whack a Zombie (2)",
                    "High Gravity (2)",
                    "Squash Showdown! 2 (2)",
                    "Zombies VS Zombies 2 (2)",
                    "Splash and Clash (2)",
                    "Melon Ninja (2)",
                    "Eclipse (2)",
                    "Wall-nut Bowling (2)",
                    "Big Trouble Little Zombie (2)",
                    "True Art is an Explosion 2 (2)",
                    "Graveout (2)",
                    "The Floor is Lava (2)",
                    "Art Challenge: Wall-nut (2)",
                    "Beghouled 2: Botany Crush (2)",
                    )

        if options.minigame_sanity>1:
            create_locs(regMini,
                    "Scaredy's Dream (1)",
                    "Pole Vaulting Disco (1)",
                    "Compact Planting (1)",
                    "D-Day (1)",
                    "Columns Like You See 'Em (1)",
                    "Mirrors Like You See 'Em (1)",
                    "It's Raining Seeds (1)",
                    "Last Stand (1)",
                    "Air Raid (1)",
                    "Advanced Challenge: 12-Lane Day (1)",
                    "Advanced Challenge: 12-Lane Pool (1)",
                    "True Art is an Explosion! (1)",
                    "Attack on Gargantuar! (1)",
                    "Zum-nut! (1)",
                    "Squash Showdown! (1)",
                    "Hypno-nut (1)",
                    "Dr Zomboss' Revenge (1)",
                    "Protect the Gold Magnet (1)",
                    "Compact Planting 2 (1)",
                    "Wall-nut Billiards 2 (1)",
                    "Wall-nut Billiards 3 (1)",
                    "Zombie Nimble Zombie Quick (1)",
                    "Chomper Snake (1)",
                    "Chinese Chezz (1)",
                    "2048: Pea-volution (1)",
                    "Iceborg Executrix's Revenge (1)",
                    "Capture the Flag (1)",
                    "Attack on Gargantuar! 2 (1)",
                    "Graveout 2 (1)",
                    "I, Zombie (Minigame) (1)",
                    "Archduke's Revenge (1)",
                    "Nut-o-mite (1)",

                    "Scaredy's Dream (2)",
                    "Pole Vaulting Disco (2)",
                    "D-Day (2)",
                    "Columns Like You See 'Em (2)",
                    "Mirrors Like You See 'Em (2)",
                    "It's Raining Seeds (2)",
                    "Last Stand (2)",
                    "Air Raid (2)",
                    "Advanced Challenge: 12-Lane Day (2)",
                    "Advanced Challenge: 12-Lane Pool (2)",
                    "True Art is an Explosion! (2)",
                    "Attack on Gargantuar! (2)",
                    "Zum-nut! (2)",
                    "Squash Showdown! (2)",
                    "Hypno-nut (2)",
                    "Dr Zomboss' Revenge (2)",
                    "Protect the Gold Magnet (2)",
                    "Compact Planting 2 (2)",
                    "Wall-nut Billiards 2 (2)",
                    "Wall-nut Billiards 3 (2)",
                    "Zombie Nimble Zombie Quick (2)",
                    "Chomper Snake (2)",
                    "Chinese Chezz (2)",
                    "2048: Pea-volution (2)",
                    "Iceborg Executrix's Revenge (2)",
                    "Capture the Flag (2)",
                    "Attack on Gargantuar! 2 (2)",
                    "Graveout 2 (2)",
                    "I, Zombie (Minigame) (2)",
                    "Archduke's Revenge (2)",

                    "Nut-o-mite (2)"

                        )


    regVB = create_region("Vasebreaker",player, world)
    if options.vasebreaker_sanity:
        create_locs(regVB,"Vasebreaker (1)",
    "Vasebreaker 2 (1)",
    "Chain Reaction (1)",
    "Vasebreaker (2)",
    "Vasebreaker 2 (2)",
    "Chain Reaction (2)")


    regSu = create_region("Survival",player, world)
    if options.survival_sanity:
        create_locs(regVB,
                    "Survival: Day (1)",
        "Survival: Day (Hard) (1)",
        "Survival: Night (1)",
        "Survival: Night (Hard) (1)",
        "Survival: Pool (1)",
        "Survival: Pool (Hard) (1)",
        "Survival: Fog (1)",
        "Survival: Fog (Hard) (1)",
        "Survival: Roof (1)",
        "Survival: Roof (Hard) (1)",

        "Survival: Day (2)",
        "Survival: Day (Hard) (2)",
        "Survival: Night (2)",
        "Survival: Night (Hard) (2)",
        "Survival: Pool (2)",
        "Survival: Pool (Hard) (2)",
        "Survival: Fog (2)",
        "Survival: Fog (Hard) (2)",
        "Survival: Roof (2)",
        "Survival: Roof (Hard) (2)",
        )



    regOM = create_region("Odyssey Menu", player, world)


    if options.adventure_odyssey or options.goal_type == 2:
            create_locs(regOM,
            "Odyssey Adventure: Level 1 (1)",
            "Odyssey Adventure: Level 2 (1)",
            "Odyssey Adventure: Level 3 (1)",
            "Odyssey Adventure: Level 4 (1)",
            "Odyssey Adventure: Level 5 (1)",
            "Odyssey Adventure: Level 6 (1)",
            "Odyssey Adventure: Level 7 (1)",
            "Odyssey Adventure: Level 8 (1)",
            "Odyssey Adventure: Level 9 (1)",
            "Odyssey Adventure: Level 10 (1)",
            "Odyssey Adventure: Level 11 (1)",
            "Odyssey Adventure: Level 12 (1)",
            "Odyssey Adventure: Level 13 (1)",
            "Odyssey Adventure: Level 14 (1)",
            "Odyssey Adventure: Level 15 (1)",

            "Odyssey Adventure: Level 1 (2)",
            "Odyssey Adventure: Level 2 (2)",
            "Odyssey Adventure: Level 3 (2)",
            "Odyssey Adventure: Level 4 (2)",
            "Odyssey Adventure: Level 5 (2)",
            "Odyssey Adventure: Level 6 (2)",
            "Odyssey Adventure: Level 7 (2)",
            "Odyssey Adventure: Level 8 (2)",
            "Odyssey Adventure: Level 9 (2)",
            "Odyssey Adventure: Level 10 (2)",
            "Odyssey Adventure: Level 11 (2)",
            "Odyssey Adventure: Level 12 (2)",
            "Odyssey Adventure: Level 13 (2)",
            "Odyssey Adventure: Level 14 (2)",
            "Odyssey Adventure: Level 15 (2)")



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
