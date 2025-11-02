from typing import NamedTuple

from BaseClasses import Item, ItemClassification


class PVZFItem(Item):
    game: str = "Plants Vs Zombies Fusion"


class PVZFItemData(NamedTuple):
    code: int | None = None
    classification: ItemClassification = ItemClassification.progression


generic_item_data_table: dict[str, PVZFItemData] = {
    "Seed Slot": PVZFItemData(201, ItemClassification.progression),
    "Bonus Sun": PVZFItemData(200, ItemClassification.filler),
    "Zen Garden Plant": PVZFItemData(202, ItemClassification.filler),
    "Diamond": PVZFItemData(203, ItemClassification.filler),
    #"Free Sunflowers": PVZFItemData(204, ItemClassification.filler),
    "Star Meteor": PVZFItemData(205, ItemClassification.filler),
    #"Bonus Starting Sun": PVZFItemData(202, ItemClassification.useful),
    #"Lawn Clear":PVZFItemData(201, ItemClassification.filler),
    # refreshed cooldowns

}
traps_item_data_table:dict[str, PVZFItemData] = {
#"Placebo Trap": PVZFItemData(220, ItemClassification.trap),
"10x Game Speed": PVZFItemData(220, ItemClassification.trap),
"Destroy Everything": PVZFItemData(221, ItemClassification.trap),
"Rough Economy": PVZFItemData(222, ItemClassification.trap),
"The Fog is Coming": PVZFItemData(223, ItemClassification.trap),
#Queen Jack in the box

#Sun loss
#lawn shuffle - randomize every plants location
#Unplant Lawn - turn every plant on the lawn into a droppedcard
#extra flag - pushes progress bar back a flag
#conveyor belt trap
#bungee ambush

#seed packet shuffle

#grave danger
#freezing trap
#ds sounds
    #lets go gambling
}

access_item_table: dict[str, PVZFItemData] = {
    "Day Access": PVZFItemData(90, ItemClassification.progression),
    "Night Access": PVZFItemData(91, ItemClassification.progression),
    "Pool Access": PVZFItemData(92, ItemClassification.progression),
    "Fog Access": PVZFItemData(93, ItemClassification.progression),
    "Roof Access": PVZFItemData(94, ItemClassification.progression),
    "Snow Access": PVZFItemData(95, ItemClassification.progression),
    "Fusion Challenge Access": PVZFItemData(96, ItemClassification.progression),
    "Fusion Showcase Access": PVZFItemData(97, ItemClassification.progression),
    "Odyssey Mode": PVZFItemData(98, ItemClassification.progression),
    "Abyss Mode": PVZFItemData(99, ItemClassification.progression),
    "TD Day Access": PVZFItemData(100, ItemClassification.progression),
    "TD Night Access": PVZFItemData(101, ItemClassification.progression),
    "TD Pool Access": PVZFItemData(102, ItemClassification.progression),
    #"TD Fog Access": PVZFItemData(103, ItemClassification.progression),
    #"TD Roof Access": PVZFItemData(104, ItemClassification.progression),
    "Fusion Showcase Day Plants": PVZFItemData(105, ItemClassification.progression),
    "Fusion Showcase Night Plants": PVZFItemData(106, ItemClassification.progression),
    "Fusion Showcase Pool Plants": PVZFItemData(107, ItemClassification.progression),
    "Fusion Showcase Fog Plants": PVZFItemData(108, ItemClassification.progression),
    "Fusion Showcase Roof Plants": PVZFItemData(109, ItemClassification.progression),

}


plants_item_data_table: dict[str, PVZFItemData] = {
    "Peashooter": PVZFItemData(1, ItemClassification.progression),
    "Sunflower":PVZFItemData(2, ItemClassification.progression),
    "Cherry Bomb":PVZFItemData(3, ItemClassification.progression),
    "Wall-nut": PVZFItemData(4, ItemClassification.progression),
    "Potato Mine": PVZFItemData(5, ItemClassification.progression),
    "Chomper": PVZFItemData(6, ItemClassification.progression),
    "Plant Giftbox": PVZFItemData(7, ItemClassification.useful),
    "Tall-nut": PVZFItemData(8, ItemClassification.progression),
    "EndoFlame": PVZFItemData(9, ItemClassification.useful),
    "Puff-shroom": PVZFItemData(10, ItemClassification.progression),
    "Fume-shroom": PVZFItemData(11, ItemClassification.progression),
    "Hypno-shroom": PVZFItemData(12, ItemClassification.progression),
    "Scaredy-shroom": PVZFItemData(13, ItemClassification.progression),
    "Ice-shroom": PVZFItemData(14, ItemClassification.progression),
    "Doom-shroom": PVZFItemData(15, ItemClassification.progression),
    "Zombie Giftbox": PVZFItemData(16, ItemClassification.useful),
    "Gloom-shroom": PVZFItemData(17, ItemClassification.progression),
    "Grave Buster": PVZFItemData(18, ItemClassification.useful),
    "Lily Pad": PVZFItemData(19, ItemClassification.progression),
    "Squash": PVZFItemData(20, ItemClassification.progression),
    "Threepeater": PVZFItemData(21, ItemClassification.progression),
    "Tangle Kelp": PVZFItemData(22, ItemClassification.progression),
    "Jalapeno": PVZFItemData(23, ItemClassification.progression),
    "Spikeweed": PVZFItemData(24, ItemClassification.progression),
    "Torchwood": PVZFItemData(25, ItemClassification.progression),
    "Spikerock": PVZFItemData(26, ItemClassification.progression),
    "Barley": PVZFItemData(27, ItemClassification.useful),
    "Sea-shroom": PVZFItemData(28, ItemClassification.progression),
    "Plantern": PVZFItemData(29, ItemClassification.progression),
    "Cactus": PVZFItemData(30, ItemClassification.progression),
    "Blover": PVZFItemData(31, ItemClassification.progression),
    "Starfruit": PVZFItemData(32, ItemClassification.progression),
    "Pumpkin": PVZFItemData(33, ItemClassification.progression),
    "Magnet-shroom": PVZFItemData(34, ItemClassification.progression),
    "Cattail": PVZFItemData(35, ItemClassification.progression),
    "Imitater": PVZFItemData(36, ItemClassification.useful),
    "Cabbage-pult": PVZFItemData(37, ItemClassification.progression),
    "Flower Pot": PVZFItemData(38, ItemClassification.progression),
    "Kernel-pult": PVZFItemData(39, ItemClassification.progression),
    "Garlic": PVZFItemData(40, ItemClassification.progression),
    "Umbrella Leaf": PVZFItemData(41, ItemClassification.progression),
    "Marigold": PVZFItemData(42, ItemClassification.progression),
    "Melon-pult": PVZFItemData(43, ItemClassification.progression),
    "Cob Cannon": PVZFItemData(44, ItemClassification.progression),
    "Jicamagic": PVZFItemData(45, ItemClassification.progression),
    "Firnace": PVZFItemData(46, ItemClassification.progression),
    "Spruce Sharpshooter": PVZFItemData(47, ItemClassification.progression),
    "Saw-me-not": PVZFItemData(48, ItemClassification.progression),
    "Snow Lotus": PVZFItemData(49, ItemClassification.progression),
    "Aloe Aqua": PVZFItemData(50, ItemClassification.progression),
    "Bamblock": PVZFItemData(51, ItemClassification.progression),
    "Frozen Giftbox": PVZFItemData(52, ItemClassification.useful),
    "Spruce Ballista": PVZFItemData(53, ItemClassification.progression),
    #"???": PVZFItemData(54, ItemClassification.filler),

    "Cattail Girl": PVZFItemData(55, ItemClassification.progression),
    "Swordmaster Starfruit": PVZFItemData(56, ItemClassification.progression),
    "Nyan Squash": PVZFItemData(57, ItemClassification.progression),
    "Burger Blaster": PVZFItemData(58, ItemClassification.progression),
    "Queen Endoflame": PVZFItemData(59, ItemClassification.progression),#odyssey exclusive
    "Coldsnap Bean": PVZFItemData(60, ItemClassification.progression),
    "Amp-nion": PVZFItemData(61, ItemClassification.progression),
    "Sniper Pea": PVZFItemData(62, ItemClassification.progression),#odyssey exclusive
    "Chrysanctum": PVZFItemData(63, ItemClassification.progression),
    "Icetip Lily": PVZFItemData(64, ItemClassification.progression),
    "Pearmafrost": PVZFItemData(65, ItemClassification.progression),
    "Doubleblast Passionfruit": PVZFItemData(66, ItemClassification.progression),
    "Lucky Blover": PVZFItemData(67, ItemClassification.useful),

}

td_plants_item_data_table: dict[str, PVZFItemData] = {
    "Solar Shooter (TD)": PVZFItemData(500, ItemClassification.progression),
    "Sunrise-shroom (TD)": PVZFItemData(501, ItemClassification.progression),
    "Explod-o-shooter (TD)": PVZFItemData(502, ItemClassification.progression),
    "Star-nut (TD)": PVZFItemData(503, ItemClassification.progression),
    "Solar Mine (TD)": PVZFItemData(504, ItemClassification.progression),
    "Frost Gloom-shroom (TD)": PVZFItemData(505, ItemClassification.progression),
    "Cherry Chomper (TD)": PVZFItemData(506, ItemClassification.progression),
    "Amp-nion (TD)": PVZFItemData(507, ItemClassification.progression),
    "Pea-shroom (TD)": PVZFItemData(508, ItemClassification.progression),
    "Sun-shroom (TD)": PVZFItemData(509, ItemClassification.progression),
    "Soot-shroom (TD)": PVZFItemData(510, ItemClassification.progression),
    "Grave Buster (TD)": PVZFItemData(511, ItemClassification.progression),
    "Gutsy-shroom (TD)": PVZFItemData(512, ItemClassification.progression),
    "Breeze Blover (TD)": PVZFItemData(513, ItemClassification.progression),
    "Death Star (TD)": PVZFItemData(514, ItemClassification.progression),
    "Drippy Pitcher (TD)": PVZFItemData(515, ItemClassification.progression),
}

tools_item_data_table: dict[str, PVZFItemData] = {
    "Shovel": PVZFItemData(70, ItemClassification.useful),
    "Fertilizer": PVZFItemData(71, ItemClassification.progression),
    "Plant Gloves":PVZFItemData(72, ItemClassification.progression),
    #"Zombie Gloves": PVZFItemData(73, ItemClassification.useful),
    "Mallet": PVZFItemData(74, ItemClassification.progression),

    #"Almanac": PVZFItemData(75, ItemClassification.filler),
    "Watering Can": PVZFItemData(76, ItemClassification.filler),
    "Phonograph": PVZFItemData(77, ItemClassification.filler),
    "Bug Spray": PVZFItemData(78, ItemClassification.filler),
    "Wheelbarrow": PVZFItemData(79, ItemClassification.filler),
    #"Advanced Fusions": PVZFItemData(80, ItemClassification.progression),
    #"Lawnmowers": PVZFItemData(159, ItemClassification.progression),
    #"Pool Cleaners": PVZFItemData(160, ItemClassification.progression),




}

minigame_item_table: dict[str, PVZFItemData] = {

    "Scaredy's Dream": PVZFItemData(110, ItemClassification.progression),
    "Pole Vaulting Disco": PVZFItemData(111, ItemClassification.progression),
    "Compact Planting": PVZFItemData(112, ItemClassification.progression),
    "Newspaper War": PVZFItemData(113, ItemClassification.progression),
    "D-Day": PVZFItemData(114, ItemClassification.progression),
    #"ZombiesTD": PVZFItemData(115, ItemClassification.progression),
    #"ZombiesTD 2": PVZFItemData(116, ItemClassification.progression),
    "Matryoshka": PVZFItemData(115, ItemClassification.progression),
    "Columns Like You See 'Em": PVZFItemData(116, ItemClassification.progression),
    "Mirrors Like You See 'Em": PVZFItemData(117, ItemClassification.progression),
    "It's Raining Seeds": PVZFItemData(118, ItemClassification.progression),
    "Last Stand": PVZFItemData(119, ItemClassification.progression),
    "Air Raid": PVZFItemData(120, ItemClassification.progression),
    "Advanced Challenge: 12-Lane Day": PVZFItemData(121, ItemClassification.progression),
    "Advanced Challenge: 12-Lane Pool": PVZFItemData(122, ItemClassification.progression),
    #"ZombiesTD 3": PVZFItemData(125, ItemClassification.progression),
    "Level Edtior": PVZFItemData(123, ItemClassification.filler),
    "Custom Level": PVZFItemData(124, ItemClassification.filler),


    "True Art is an Explosion!": PVZFItemData(125, ItemClassification.progression),
    "Pogo Party!": PVZFItemData(126, ItemClassification.progression),
    "Attack on Gargantuar!": PVZFItemData(127, ItemClassification.progression),
    "Zum-nut!": PVZFItemData(128, ItemClassification.progression),
    "Squash Showdown!": PVZFItemData(129, ItemClassification.progression),
    "Hypno-tism!": PVZFItemData(130, ItemClassification.progression),
    "Dr Zomboss' Revenge": PVZFItemData(131, ItemClassification.progression),
    "Protect the Gold Magnet": PVZFItemData(132, ItemClassification.progression),
    "Compact Planting 2": PVZFItemData(133, ItemClassification.progression),
    "Bungee Blitz": PVZFItemData(134, ItemClassification.progression),
    "Beghouled": PVZFItemData(135, ItemClassification.progression),
    "Seeing Stars": PVZFItemData(136, ItemClassification.progression),
    "Wall-nut Billiards": PVZFItemData(137, ItemClassification.progression),
    "Wall-nut Billiards 2": PVZFItemData(138, ItemClassification.progression),
    "Wall-nut Billiards 3": PVZFItemData(139, ItemClassification.progression),
    "Whack a Zombie": PVZFItemData(140, ItemClassification.progression),
    "Zombie Nimble Zombie Quick": PVZFItemData(141, ItemClassification.progression),
    "High Gravity": PVZFItemData(142, ItemClassification.progression),

    "Chomper Snake": PVZFItemData(143, ItemClassification.progression),
    "Chinese Chezz": PVZFItemData(144, ItemClassification.progression),
    "Squash Showdown 2": PVZFItemData(145, ItemClassification.progression),
    "Zombies VS Zombies 2": PVZFItemData(146, ItemClassification.progression),#yes this is a normal minigame not odyssey for some reason
    "2048: Pea-volution": PVZFItemData(147, ItemClassification.progression),
    "Splash and Clash": PVZFItemData(148, ItemClassification.progression),
    "Melon Ninja": PVZFItemData(149, ItemClassification.progression),
    "Eclipse": PVZFItemData(150, ItemClassification.progression),
    "Wall-nut Bowling": PVZFItemData(151, ItemClassification.progression),
    "Iceborg Executrix's Revenge": PVZFItemData(152, ItemClassification.progression),
    "Big Trouble Little Zombie": PVZFItemData(153, ItemClassification.progression),
    "True Art is an Explosion 2": PVZFItemData(154, ItemClassification.progression),
    "Capture the Flag": PVZFItemData(155, ItemClassification.progression),
    "Attack on Gargantuar! 2": PVZFItemData(156, ItemClassification.progression),

    "Graveout": PVZFItemData(157, ItemClassification.progression),
    "Graveout Endless": PVZFItemData(158, ItemClassification.filler),
    "Graveout 2": PVZFItemData(159, ItemClassification.progression),
    "The Floor is Lava": PVZFItemData(160, ItemClassification.progression),
    "Art Challenge: Wall-nut": PVZFItemData(161, ItemClassification.progression),
    "I, Zombie (Minigame)": PVZFItemData(162, ItemClassification.progression),


}

progressives_item_data_table: dict[str, PVZFItemData] = {

    #"Progressive Compact Planting": PVZFItemData(154, ItemClassification.progression),
    #"Progressive Wall-nut Billiards": PVZFItemData(156, ItemClassification.progression),
    #"Progressive Squash Showdown": PVZFItemData(157, ItemClassification.progression),

    #odyssey exclusive
    #"Progressive Wack-a-Zombie": PVZFItemData(158, ItemClassification.progression),
    #"Progressive Zombies VS Zombies": PVZFItemData(159, ItemClassification.progression),
}


item_data_table = {
    **generic_item_data_table,**traps_item_data_table,**plants_item_data_table,**tools_item_data_table,**access_item_table,**minigame_item_table
}

item_table = {name: data.code for name, data in item_data_table.items() if data.code is not None}
