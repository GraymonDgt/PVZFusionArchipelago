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

    "Day: Level 1 (2)": 101,
    "Day: Level 2 (2)": 102,
    "Day: Level 3 (2)": 103,
    "Day: Level 4 (2)": 104,
    "Day: Level 5 (2)": 105,
    "Day: Level 6 (2)": 106,
    "Day: Level 7 (2)": 107,
    "Day: Level 8 (2)": 108,
    "Day: Level 9 (2)": 109,

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

    "Night: Level 10 (2)": 110,
    "Night: Level 11 (2)": 111,
    "Night: Level 12 (2)": 112,
    "Night: Level 13 (2)": 113,
    "Night: Level 14 (2)": 114,
    "Night: Level 15 (2)": 115,
    "Night: Level 16 (2)": 116,
    "Night: Level 17 (2)": 117,
    "Night: Level 18 (2)": 118,


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

    "Pool: Level 19 (2)": 119,
    "Pool: Level 20 (2)": 120,
    "Pool: Level 21 (2)": 121,
    "Pool: Level 22 (2)": 122,
    "Pool: Level 23 (2)": 123,
    "Pool: Level 24 (2)": 124,
    "Pool: Level 25 (2)": 125,
    "Pool: Level 26 (2)": 126,
    "Pool: Level 27 (2)": 127,


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

    "Fog: Level 28 (2)": 128,
    "Fog: Level 29 (2)": 129,
    "Fog: Level 30 (2)": 130,
    "Fog: Level 31 (2)": 131,
    "Fog: Level 32 (2)": 132,
    "Fog: Level 33 (2)": 133,
    "Fog: Level 34 (2)": 134,
    "Fog: Level 35 (2)": 135,
    "Fog: Level 36 (2)": 136,
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

    "Roof: Level 37 (2)": 137,
    "Roof: Level 38 (2)": 138,
    "Roof: Level 39 (2)": 139,
    "Roof: Level 40 (2)": 140,
    "Roof: Level 41 (2)": 141,
    "Roof: Level 42 (2)": 142,
    "Roof: Level 43 (2)": 143,
    "Roof: Level 44 (2)": 144,
    "Roof: Level 45 (2)": 145,

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

    "Snow: Level 1 (2)": 146,
    "Snow: Level 2 (2)": 147,
    "Snow: Level 3 (2)": 148,
    "Snow: Level 4 (2)": 149,
    "Snow: Level 5 (2)": 150,
    "Snow: Level 6 (2)": 151,
    "Snow: Level 7 (2)": 152,
    "Snow: Level 8 (2)": 153,
    "Snow: Level 9 (2)": 154,
}

FUSCHAL_table = {

    "Fusion Challenge: Explod-o-shooter": 55,
    "Fusion Challenge: Chompzilla": 56,
    "Fusion Challenge: Charm-shroom": 57,
    "Fusion Challenge: Doomspike-shroom": 58,
    "Fusion Challenge: Infernowood": 59,
    "Fusion Challenge: Krakerberus": 60,
    "Fusion Challenge: Stardrop": 61,
    "Fusion Challenge: Bloverthorn Pumpkin": 62,
    "Fusion Challenge: Salad-pult": 63,
    "Fusion Challenge: Alchemist Umbrella": 64,
    "Fusion Challenge: Spruce Supershooter": 65,
}



MINIGAME_table = {

    "Dr. Zomboss' Revenge": 100


}



# TODO shields, act clears
# Correspond to 3626000 + course index * 7 + star index, then secret stars, then keys, then 100 Coin Stars
location_table = {**DAY_table,**NIGHT_table,**POOL_table,**FOG_table,**ROOF_table,**SNOW_table,**FUSCHAL_table,**MINIGAME_table}
