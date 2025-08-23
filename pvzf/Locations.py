from BaseClasses import Location

class PVZFLocation(Location):
    game: str = "Plants Vs Zombies Fusion"

#Bob-omb Battlefield
DAY_table = {

    "Day: Level 1 (1)": 1,
    "Day: Level 2 (1)": 2,
    "Day: Level 3 (1)": 3,
    "Day: Level 4 (1)": 4,
    "Day: Level 5 (1)": 5,
    "Day: Level 6 (1)": 6,
    "Day: Level 7 (1)": 7,
    "Day: Level 8 (1)": 8,
    "Day: Level 9 (1)": 9,

    "Day: Level 1 (2)": 301,
    "Day: Level 2 (2)": 302,
    "Day: Level 3 (2)": 303,
    "Day: Level 4 (2)": 304,
    "Day: Level 5 (2)": 305,
    "Day: Level 6 (2)": 306,
    "Day: Level 7 (2)": 307,
    "Day: Level 8 (2)": 308,
    "Day: Level 9 (2)": 309,

}

NIGHT_table = {

    "Night: Level 10 (1)": 10,
    "Night: Level 11 (1)": 11,
    "Night: Level 12 (1)": 12,
    "Night: Level 13 (1)": 13,
    "Night: Level 14 (1)": 14,
    "Night: Level 15 (1)": 15,
    "Night: Level 16 (1)": 16,
    "Night: Level 17 (1)": 17,
    "Night: Level 18 (1)": 18,

    "Night: Level 10 (2)": 310,
    "Night: Level 11 (2)": 311,
    "Night: Level 12 (2)": 312,
    "Night: Level 13 (2)": 313,
    "Night: Level 14 (2)": 314,
    "Night: Level 15 (2)": 315,
    "Night: Level 16 (2)": 316,
    "Night: Level 17 (2)": 317,
    "Night: Level 18 (2)": 318,


}

POOL_table = {

    "Pool: Level 19 (1)": 19,
    "Pool: Level 20 (1)": 20,
    "Pool: Level 21 (1)": 21,
    "Pool: Level 22 (1)": 22,
    "Pool: Level 23 (1)": 23,
    "Pool: Level 24 (1)": 24,
    "Pool: Level 25 (1)": 25,
    "Pool: Level 26 (1)": 26,
    "Pool: Level 27 (1)": 27,

    "Pool: Level 19 (2)": 319,
    "Pool: Level 20 (2)": 320,
    "Pool: Level 21 (2)": 321,
    "Pool: Level 22 (2)": 322,
    "Pool: Level 23 (2)": 323,
    "Pool: Level 24 (2)": 324,
    "Pool: Level 25 (2)": 325,
    "Pool: Level 26 (2)": 326,
    "Pool: Level 27 (2)": 327,


}
FOG_table = {

    "Fog: Level 28 (1)": 28,
    "Fog: Level 29 (1)": 29,
    "Fog: Level 30 (1)": 30,
    "Fog: Level 31 (1)": 31,
    "Fog: Level 32 (1)": 32,
    "Fog: Level 33 (1)": 33,
    "Fog: Level 34 (1)": 34,
    "Fog: Level 35 (1)": 35,
    "Fog: Level 36 (1)": 36,

    "Fog: Level 28 (2)": 328,
    "Fog: Level 29 (2)": 329,
    "Fog: Level 30 (2)": 330,
    "Fog: Level 31 (2)": 331,
    "Fog: Level 32 (2)": 332,
    "Fog: Level 33 (2)": 333,
    "Fog: Level 34 (2)": 334,
    "Fog: Level 35 (2)": 335,
    "Fog: Level 36 (2)": 336,
}
ROOF_table = {
    "Roof: Level 37 (1)": 37,
    "Roof: Level 38 (1)": 38,
    "Roof: Level 39 (1)": 39,
    "Roof: Level 40 (1)": 40,
    "Roof: Level 41 (1)": 41,
    "Roof: Level 42 (1)": 42,
    "Roof: Level 43 (1)": 43,
    "Roof: Level 44 (1)": 44,
    "Roof: Level 45 (1)": 45,

    "Roof: Level 37 (2)": 337,
    "Roof: Level 38 (2)": 338,
    "Roof: Level 39 (2)": 339,
    "Roof: Level 40 (2)": 340,
    "Roof: Level 41 (2)": 341,
    "Roof: Level 42 (2)": 342,
    "Roof: Level 43 (2)": 343,
    "Roof: Level 44 (2)": 344,
    "Roof: Level 45 (2)": 345,

}

SNOW_table = {
    "Snow: Level 1 (1)": 46,
    "Snow: Level 2 (1)": 47,
    "Snow: Level 3 (1)": 48,
    "Snow: Level 4 (1)": 49,
    "Snow: Level 5 (1)": 50,
    "Snow: Level 6 (1)": 51,
    "Snow: Level 7 (1)": 52,
    "Snow: Level 8 (1)": 53,
    "Snow: Level 9 (1)": 54,

    "Snow: Level 1 (2)": 346,
    "Snow: Level 2 (2)": 347,
    "Snow: Level 3 (2)": 348,
    "Snow: Level 4 (2)": 349,
    "Snow: Level 5 (2)": 350,
    "Snow: Level 6 (2)": 351,
    "Snow: Level 7 (2)": 352,
    "Snow: Level 8 (2)": 353,
    "Snow: Level 9 (2)": 354,
}

FUSCHAL_table = {
    "Fusion Challenge: Explod-o-shooter (1)": 55,
    "Fusion Challenge: Chompzilla (1)": 56,
    "Fusion Challenge: Charm-shroom (1)": 57,
    "Fusion Challenge: Doomspike-shroom (1)": 58,
    "Fusion Challenge: Infernowood (1)": 59,
    "Fusion Challenge: Krakerberus (1)": 60,
    "Fusion Challenge: Stardrop (1)": 61,
    "Fusion Challenge: Bloverthorn Pumpkin (1)": 62,
    "Fusion Challenge: Salad-pult (1)": 63,
    "Fusion Challenge: Alchemist Umbrella (1)": 64,
    "Fusion Challenge: Spruce Supershooter (1)": 65,

    "Fusion Challenge: Explod-o-shooter (2)": 355,
    "Fusion Challenge: Chompzilla (2)": 356,
    "Fusion Challenge: Charm-shroom (2)": 357,
    "Fusion Challenge: Doomspike-shroom (2)": 358,
    "Fusion Challenge: Infernowood (2)": 359,
    "Fusion Challenge: Krakerberus (2)": 360,
    "Fusion Challenge: Stardrop (2)": 361,
    "Fusion Challenge: Bloverthorn Pumpkin (2)": 362,
    "Fusion Challenge: Salad-pult (2)": 363,
    "Fusion Challenge: Alchemist Umbrella (2)": 364,
    "Fusion Challenge: Spruce Supershooter (2)": 365,
}

FUSSHOW_table = {
    "Fusion Showcase: Titan Pea Turret (1)": 66,
    "Fusion Showcase: Explod-o-tato Mine (1)": 67,
    "Fusion Showcase: Pumpkin Bunker (1)": 68,
    "Fusion Showcase: Nugget-shroom (1)": 69,
    "Fusion Showcase: Spuddy-shroom (1)": 70,
    "Fusion Showcase: Chomper Maw (1)": 71,
    "Fusion Showcase: Foul-shroom (1)": 72,
    "Fusion Showcase: Mind-blover (1)": 73,
    "Fusion Showcase: Boomwood (1)": 74,
    "Fusion Showcase: Bamboom (1)": 75,
    "Fusion Showcase: Spike-nut (1)": 76,
    "Fusion Showcase: Leviathan-shroom (1)": 77,

    "Fusion Showcase: Titan Pea Turret (2)": 366,
    "Fusion Showcase: Explod-o-tato Mine (2)": 367,
    "Fusion Showcase: Pumpkin Bunker (2)": 368,
    "Fusion Showcase: Nugget-shroom (2)": 369,
    "Fusion Showcase: Spuddy-shroom (2)": 370,
    "Fusion Showcase: Chomper Maw (2)": 371,
    "Fusion Showcase: Foul-shroom (2)": 372,
    "Fusion Showcase: Mind-blover (2)": 373,
    "Fusion Showcase: Boomwood (2)": 374,
    "Fusion Showcase: Bamboom (2)": 375,
    "Fusion Showcase: Spike-nut (2)": 376,
    "Fusion Showcase: Leviathan-shroom (2)": 377,

}

#MINIG_table = {
#}




MINIGAME_table = {

    "Dr. Zomboss' Revenge": 100


}



# TODO shields, act clears
# Correspond to 3626000 + course index * 7 + star index, then secret stars, then keys, then 100 Coin Stars
location_table = {**DAY_table,**NIGHT_table,**POOL_table,**FOG_table,**ROOF_table,**SNOW_table,**FUSCHAL_table,**MINIGAME_table,**FUSSHOW_table}
