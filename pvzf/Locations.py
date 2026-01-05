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
    "Fusion Challenge: Jicamagic (1)": 66,

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
    "Fusion Challenge: Jicamagic (2)": 366,
}

FUSSHOW_table = {

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
NEWFUSSHOW_table = {






}


#MINIG_table = {
#}
TENFLAG_table = {
"Ten-Flag Day (1)",
"Ten-Flag Night (1)",
"Ten-Flag Pool (1)",
"Ten-Flag Fog (1)",
"Ten-Flag Roof (1)",
"Ten-Flag Zombie Gacha (1)",
"Ten-Flag Zombotany (1)",
"Ten-Flag Night Disco (1)",
"Ten-Flag Scaredy's Dream (1)",
"Ten-Flag Plant Gacha (1)",
"Ten-Flag Equivalent Exchange (1)",
"Ten-Flag Multi Gacha (1)",

"Ten-Flag Day (2)",
"Ten-Flag Night (2)",
"Ten-Flag Pool (2)",
"Ten-Flag Fog (2)",
"Ten-Flag Roof (2)",
"Ten-Flag Zombie Gacha (2)",
"Ten-Flag Zombotany (2)",
"Ten-Flag Night Disco (2)",
"Ten-Flag Scaredy's Dream (2)",
"Ten-Flag Plant Gacha (2)",
"Ten-Flag Equivalent Exchange (2)",
"Ten-Flag Multi Gacha (2)",

}



MINIGAME_table = {
    "Scaredy's Dream (1)":80,
    "Pole Vaulting Disco (1)":81,
    "Compact Planting (1)":82,
    "Newspaper War (1)":83,
    "D-Day (1)":84,
    "Matryoshka (1)":85,
    "Columns Like You See 'Em (1)":86,
    "Mirrors Like You See 'Em (1)":87,
    "It's Raining Seeds (1)":88,
    "Last Stand (1)":89,
    "Air Raid (1)":90,
    "Advanced Challenge: 12-Lane Day (1)":91,
    "Advanced Challenge: 12-Lane Pool (1)":92,
    "True Art is an Explosion! (1)":93,
    "Pogo Party! (1)":94,
    "Attack on Gargantuar! (1)":95,
    "Zum-nut! (1)":96,
    "Squash Showdown! (1)":97,
    "Hypno-nut (1)":98,
    "Dr Zomboss' Revenge (1)":99,
    "Protect the Gold Magnet (1)":100,
    "Compact Planting 2 (1)":101,
    "Bungee Blitz (1)":102,
    "Beghouled (1)":103,
    "Seeing Stars (1)":104,
    "Wall-nut Billiards (1)":105,
    "Wall-nut Billiards 2 (1)":106,
    "Wall-nut Billiards 3 (1)":107,
    "Whack a Zombie (1)":108,
    "Zombie Nimble Zombie Quick (1)":109,
    "High Gravity (1)":110,
    "Chomper Snake (1)":111,
    "Chinese Chezz (1)":112,
    "Squash Showdown! 2 (1)":113,
    "Zombies VS Zombies 2 (1)":114,
    "2048: Pea-volution (1)":115,
    "Splash and Clash (1)":116,
    "Melon Ninja (1)":117,
    "Eclipse (1)":118,
    "Wall-nut Bowling (1)":119,
    "Iceborg Executrix's Revenge (1)":120,
    "Big Trouble Little Zombie (1)":121,
    "True Art is an Explosion 2 (1)":122,
    "Capture the Flag (1)":123,
    "Attack on Gargantuar! 2 (1)":124,

    "Graveout (1)":125,
    "Graveout 2 (1)":126,
    "The Floor is Lava (1)":127,
    "Art Challenge: Wall-nut (1)":128,
    "I, Zombie (Minigame) (1)":129,

    "Archduke's Revenge (1)":130,
    "Beghouled 2: Botany Crush (1)":131,
    "Nut-o-mite (1)":132,

    "Scaredy's Dream (2)":380,
    "Pole Vaulting Disco (2)":381,
    "Compact Planting (2)":382,
    "Newspaper War (2)":383,
    "D-Day (2)":384,
    "Matryoshka (2)":385,
    "Columns Like You See 'Em (2)":386,
    "Mirrors Like You See 'Em (2)":387,
    "It's Raining Seeds (2)":388,
    "Last Stand (2)":389,
    "Air Raid (2)":390,
    "Advanced Challenge: 12-Lane Day (2)":391,
    "Advanced Challenge: 12-Lane Pool (2)":392,
    "True Art is an Explosion! (2)":393,
    "Pogo Party! (2)":394,
    "Attack on Gargantuar! (2)":395,
    "Zum-nut! (2)":396,
    "Squash Showdown! (2)":397,
    "Hypno-nut (2)":398,
    "Dr Zomboss' Revenge (2)":399,
    "Protect the Gold Magnet (2)":400,
    "Compact Planting 2 (2)":401,
    "Bungee Blitz (2)":402,
    "Beghouled (2)":403,
    "Seeing Stars (2)":404,
    "Wall-nut Billiards (2)":405,
    "Wall-nut Billiards 2 (2)":406,
    "Wall-nut Billiards 3 (2)":407,
    "Whack a Zombie (2)":408,
    "Zombie Nimble Zombie Quick (2)":409,
    "High Gravity (2)":410,
    "Chomper Snake (2)":411,
    "Chinese Chezz (2)":412,
    "Squash Showdown! 2 (2)":413,
    "Zombies VS Zombies 2 (2)":414,
    "2048: Pea-volution (2)":415,
    "Splash and Clash (2)":416,
    "Melon Ninja (2)":417,
    "Eclipse (2)":418,
    "Wall-nut Bowling (2)":419,
    "Iceborg Executrix's Revenge (2)":420,
    "Big Trouble Little Zombie (2)":421,
    "True Art is an Explosion 2 (2)":422,
    "Capture the Flag (2)":423,
    "Attack on Gargantuar! 2 (2)":424,

    "Graveout (2)": 425,
    "Graveout 2 (2)": 426,
    "The Floor is Lava (2)": 427,
    "Art Challenge: Wall-nut (2)": 428,
    "I, Zombie (Minigame) (2)": 429,

    "Archduke's Revenge (2)": 430,
    "Beghouled 2: Botany Crush (2)": 431,
    "Nut-o-mite (2)": 432,
}

vase_table = {
    "Vasebreaker (1)": 200,
    "Vasebreaker 2 (1)": 201,
    "Chain Reaction (1)": 202,
    "Vasebreaker (2)": 500,
    "Vasebreaker 2 (2)": 501,
    "Chain Reaction (2)": 502,
}

survival_table = {
    "Survival: Day (1)": 280,
    "Survival: Day (Hard) (1)": 281,
    "Survival: Night (1)": 282,
    "Survival: Night (Hard) (1)": 283,
    "Survival: Pool (1)": 284,
    "Survival: Pool (Hard) (1)": 285,
    "Survival: Fog (1)": 286,
    "Survival: Fog (Hard) (1)": 287,
    "Survival: Roof (1)": 288,
    "Survival: Roof (Hard) (1)": 289,

    "Survival: Day (2)": 580,
    "Survival: Day (Hard) (2)": 581,
    "Survival: Night (2)": 582,
    "Survival: Night (Hard) (2)": 583,
    "Survival: Pool (2)": 584,
    "Survival: Pool (Hard) (2)": 585,
    "Survival: Fog (2)": 586,
    "Survival: Fog (Hard) (2)": 587,
    "Survival: Roof (2)": 588,
    "Survival: Roof (Hard) (2)": 589,
}



ODYSSEY_table = {
    "Odyssey Adventure: Level 1 (1)": 601,
    "Odyssey Adventure: Level 2 (1)": 602,
    "Odyssey Adventure: Level 3 (1)": 603,
    "Odyssey Adventure: Level 4 (1)": 604,
    "Odyssey Adventure: Level 5 (1)": 605,
    "Odyssey Adventure: Level 6 (1)": 606,
    "Odyssey Adventure: Level 7 (1)": 607,
    "Odyssey Adventure: Level 8 (1)": 608,
    "Odyssey Adventure: Level 9 (1)": 609,
    "Odyssey Adventure: Level 10 (1)": 610,
    "Odyssey Adventure: Level 11 (1)": 611,
    "Odyssey Adventure: Level 12 (1)": 612,
    "Odyssey Adventure: Level 13 (1)": 613,
    "Odyssey Adventure: Level 14 (1)": 614,
    "Odyssey Adventure: Level 15 (1)": 615,

    "Odyssey Adventure: Level 1 (2)": 901,
    "Odyssey Adventure: Level 2 (2)": 902,
    "Odyssey Adventure: Level 3 (2)": 903,
    "Odyssey Adventure: Level 4 (2)": 904,
    "Odyssey Adventure: Level 5 (2)": 905,
    "Odyssey Adventure: Level 6 (2)": 906,
    "Odyssey Adventure: Level 7 (2)": 907,
    "Odyssey Adventure: Level 8 (2)": 908,
    "Odyssey Adventure: Level 9 (2)": 909,
    "Odyssey Adventure: Level 10 (2)": 910,
    "Odyssey Adventure: Level 11 (2)": 911,
    "Odyssey Adventure: Level 12 (2)": 912,
    "Odyssey Adventure: Level 13 (2)": 913,
    "Odyssey Adventure: Level 14 (2)": 914,
    "Odyssey Adventure: Level 15 (2)": 915,
}

# TODO shields, act clears
# Correspond to 3626000 + course index * 7 + star index, then secret stars, then keys, then 100 Coin Stars
location_table = {**DAY_table,**NIGHT_table,**POOL_table,**FOG_table,**ROOF_table,**SNOW_table,**FUSCHAL_table,**MINIGAME_table,**FUSSHOW_table,**ODYSSEY_table,**vase_table,**survival_table}
