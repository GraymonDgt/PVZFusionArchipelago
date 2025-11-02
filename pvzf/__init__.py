import typing
import os
import json
from .Items import (item_data_table, generic_item_data_table,traps_item_data_table,plants_item_data_table,tools_item_data_table, minigame_item_table, item_table, access_item_table, PVZFItem)
from .Locations import location_table #GFZ_table, THZ_table, DSZ_table, CEZ_table,ACZ_table,RVZ_table,ERZ_table,BCZ_table,FHZ_table,PTZ_table,FFZ_table,HHZ_table,AGZ_table,ATZ_table,FFSP_table,TPSP_table,FCSP_table,CFSP_table,DWSP_table,MCSP_table,ESSP_table,BHSP_table,CCSP_table,DHSP_table,APSP_table,EXTRA_table,tokens_table,oneupcoords_table,ringmonitors_table, SRB2Location
from .Options import pvzf_options_groups,PVZFOptions
from .Rules import set_rules
from .Regions import create_regions, SRB2Zones
from BaseClasses import Item, Tutorial, ItemClassification, Region
from ..AutoWorld import World, WebWorld
import random
from multiprocessing import Process
from worlds.LauncherComponents import Component, components, Type, launch_subprocess






#class SM64Web(WebWorld):
#    tutorials = [Tutorial(
#        "Multiworld Setup Guide",
#        "A guide to setting up SM64EX for MultiWorld.",
#        "English",
#        "setup_en.md",
#        "setup/en",
#        ["N00byKing"]
#    )]

option_groups = pvzf_options_groups

#def launch_client():
#    from .Client import launch
#    launch_subprocess(launch, name="SRB2Client")


#components.append(Component(
#    "Sonic Robo Blast 2 Client",
#    "SRB2Client",
#    func=launch_client,
#    component_type=Type.CLIENT
#))

class PVZFWorld(World):
    """
    Peak game
    Remember to actually edit this later
    """
    game: str = "Plants Vs Zombies Fusion"
    topology_present = False

    item_name_to_id = item_table
    location_name_to_id = location_table

    #item_name_groups = {
    #    "Zone":zones_item_data_table,
    #    "Character":character_item_data_table,
    #    "Match Zone":mpmatch_item_table,
    #    "Trap":traps_item_data_table,
    #    "Shield":other_item_table,
    #    "Powerup":nights_item_table,
    #    "Nights Stage":special_item_data_table
    #}

    #location_name_groups = {
    #    "Greenflower Zone":GFZ_table,
    #    "Techno Hill Zone": THZ_table,
    #    "Deep Sea Zone": DSZ_table,
    #    "Castle Eggman Zone":CEZ_table,
    #    "Arid Canyon Zone":ACZ_table,
    #    "Red Volcano Zone":RVZ_table,
    #    "Egg Rock Zone":ERZ_table,
    #    "Black Core Zone":BCZ_table,
    #    "Frozen Hillside Zone":FHZ_table,
    #    "Pipe Towers Zone":PTZ_table,
    #    "Forest Fortress Zone":FFZ_table,
    #    #"Final Demo Zone": oh thats non sorted things
    #    "Haunted Heights Zone":HHZ_table,
    #    "Aerial Garden Zone":AGZ_table,
    #    "Azure Temple Zone":ATZ_table,
    #    "Floral Field Zone":FFSP_table,
    #    "Toxic Plateau Zone":TPSP_table,
    #    "Flooded Cove Zone":FCSP_table,
    #    "Cavern Fortress Zone":CFSP_table,
    #    "Dusty Wasteland Zone":DWSP_table,
    #    "Egg Satellite Zone":ESSP_table,
    #    "Black Hole Zone":BHSP_table,
    #    "Christmas Chime Zone":CCSP_table,
    #    "Dream Hill Zone":DHSP_table,
    #    "Alpine Paradise Zone":APSP_table,
    #    "Emerald Tokens":tokens_table,
    #    "1UP Monitors":oneupcoords_table,
    #    "Super Ring Monitors":ringmonitors_table
#
#
#
    #}



    required_client_version = (0, 3, 5)

    area_connections: typing.Dict[int, int]

    options_dataclass = PVZFOptions

    number_of_locations: int
    filler_count: int
    star_costs: typing.Dict[str, int]

    # Spoiler specific variable(s)
    star_costs_spoiler_key_maxlen = len(max([
        'First Floor Big Star Door',
        'Basement Big Star Door',
        'Second Floor Big Star Door',
        'MIPS 1',
        'MIPS 2',
        'Endless Stairs',
    ], key=len))


    def generate_early(self):

        max_locations = 90#TODO up this once i have enough locations
        if self.options.adventure_extra==2:
            max_locations += 18
        if self.options.challenge_sanity:
            max_locations += 24
        if self.options.showcase_sanity:
            max_locations += 176
        if self.options.minigame_sanity != 0:
            max_locations += 42
            if self.options.minigame_sanity == 2:
                max_locations += 58
        #if not self.options.time_emblems:
        #    max_locations -= 27
        self.number_of_locations = max_locations
        self.move_rando_bitvec = 0



    def create_regions(self):
        create_regions(self.multiworld, self.options, self.player)

    def set_rules(self):
        self.area_connections = {}
        set_rules(self.multiworld, self.options, self.player, self.area_connections, self.move_rando_bitvec)


    def create_item(self, name: str) -> Item:
        data = item_data_table[name]
        item = PVZFItem(name, data.classification, data.code, self.player)

        return item

    def create_items(self):
            # 1Up Mushrooms
            possible_starts = ["Peashooter", "Fume-shroom", "Scaredy-shroom", "Threepeater", "Cactus", "Starfruit",
                               "Cabbage-pult", "Kernel-pult", "Spruce Sharpshooter"]
            for i in self.options.no_start_plant:
                if i in possible_starts:
                    possible_starts.remove(i)


            rand_idx = random.randrange(len(possible_starts))

            starting_plant = possible_starts[rand_idx]


            self.multiworld.push_precollected(self.create_item("Sunflower"))
            self.multiworld.push_precollected(self.create_item(starting_plant))

            self.multiworld.push_precollected(self.create_item("Day Access"))

            slots_to_fill = self.number_of_locations

            if self.options.randomize_seed_slots:
                for i in range(9):
                    self.multiworld.itempool += [self.create_item("Seed Slot")]
                    slots_to_fill -= 1
            else:
                for i in range(9):
                    self.multiworld.push_precollected(self.create_item("Seed Slot"))


            self.multiworld.itempool += [self.create_item("Night Access")]
            self.multiworld.itempool += [self.create_item("Pool Access")]
            self.multiworld.itempool += [self.create_item("Fog Access")]
            self.multiworld.itempool += [self.create_item("Roof Access")]
            slots_to_fill -= 4
            if self.options.adventure_extra == 2:
                self.multiworld.itempool += [self.create_item("Snow Access")]
                slots_to_fill -= 1

            if self.options.challenge_sanity:
                self.multiworld.itempool += [self.create_item("Fusion Challenge Access")]
                slots_to_fill -= 1

            if self.options.showcase_sanity:
                self.multiworld.itempool += [self.create_item("Fusion Showcase Day Plants")]
                self.multiworld.itempool += [self.create_item("Fusion Showcase Night Plants")]
                self.multiworld.itempool += [self.create_item("Fusion Showcase Pool Plants")]
                self.multiworld.itempool += [self.create_item("Fusion Showcase Fog Plants")]
                self.multiworld.itempool += [self.create_item("Fusion Showcase Roof Plants")]
                slots_to_fill -= 5

            if self.options.minigame_sanity != 0:
                short_minigames = ["Compact Planting","Newspaper War", "Matryoshka", "Pogo Party!", "Bungee Blitz", "Beghouled","Seeing Stars",
                                   "Wall-nut Billiards", "Whack a Zombie", "High Gravity", "Squash Showdown 2","Zombies VS Zombies 2",
                                   "Splash and Clash", "Melon Ninja", "Eclipse", "Wall-nut Bowling","Big Trouble Little Zombie",
                                   "True Art is an Explosion 2", "Graveout", "The Floor is Lava","Art Challenge: Wall-nut"]
                for minigame in minigame_item_table.keys():

                    if self.options.minigame_sanity == 1:
                        if minigame not in short_minigames:
                            continue







                    self.multiworld.itempool += [self.create_item(minigame)]
                    slots_to_fill -= 1


            for plant in plants_item_data_table.keys():
                if plant == "Sunflower" or plant == starting_plant:
                    continue
                if plant in self.options.unique_blacklist:
                    continue
                if self.options.unique_preset == 1:
                    valid_keys = ["Cattail Girl", "Swordmaster Starfruit", "Burger Blaster",
                                  "Queen Endoflame", "Amp-nion",
                                  "Icetip Lily", "Doubleblast Passionfruit"]
                    if plant in valid_keys:
                        continue

                if self.options.unique_preset == 2:
                    valid_keys = ["Cattail Girl", "Swordmaster Starfruit", "Nyan Squash", "Burger Blaster",
                                  "Queen Endoflame", "Coldsnap Bean", "Amp-nion", "Sniper Pea", "Chrysanctum",
                                  "Icetip Lily", "Pearmafrost", "Doubleblast Passionfruit"]
                    if plant in valid_keys:
                        continue

                if self.options.adventure_extra == 0:
                    valid_keys = ["Firnace", "Spruce Sharpshooter", "Saw-me-not", "Snow Lotus",
                                  "Aloe Aqua", "Bamblock", "Frozen Giftbox", "Spruce Ballista"]
                    if plant in valid_keys:
                        continue

                self.multiworld.itempool += [self.create_item(plant)]
                slots_to_fill -=1








            for item in tools_item_data_table.keys():
                self.multiworld.itempool += [self.create_item(item)]
                slots_to_fill -=1

            if slots_to_fill!= 0:
                trap_slots = int(slots_to_fill*self.options.trap_percentage/100)
                while trap_slots != 0:
                    trapnum = random.randrange(11)
                    if trapnum<4:
                        self.multiworld.itempool += [self.create_item("10x Game Speed")]#4
                    elif trapnum<7:
                        self.multiworld.itempool += [self.create_item("Rough Economy")]
                    elif trapnum<11:
                        self.multiworld.itempool += [self.create_item("The Fog is Coming")]
                    else:
                        self.multiworld.itempool += [self.create_item("Destroy Everything")]
                    trap_slots-=1
                    slots_to_fill-=1

            if slots_to_fill != 0:
                filler_slots = slots_to_fill
                while filler_slots != 0:
                    fillernum = random.randrange(11)
                    if fillernum<4:
                        self.multiworld.itempool += [self.create_item("Bonus Sun")]
                    elif fillernum<6:
                        self.multiworld.itempool += [self.create_item("Zen Garden Plant")]
                    elif fillernum<9:
                        self.multiworld.itempool += [self.create_item("Star Meteor")]
                    else:
                        self.multiworld.itempool += [self.create_item("Diamond")]

                    filler_slots-=1
                    slots_to_fill-=1


    def generate_basic(self): #use to force items in a specific location
        #self.multiworld.get_location()
        return
           #self.multiworld.get_location("BoB: Bob-omb Buddy", self.player).place_locked_item(self.create_item("Cannon Unlock BoB"))


    def get_filler_item_name(self) -> str:
        return "Sun Bonus"

    def fill_slot_data(self):
        return {
            "CompletionType": self.options.goal_type.value,
            "RingLink": self.options.ring_link.value,
            "DeathLink": self.options.death_link.value,
            "AdventureExtra": self.options.adventure_extra.value
            #"CompletionType": self.options.completion_type.value,
        }

    def generate_output(self, output_directory: str):
        if self.multiworld.players != 1:
            return
        data = {
            "slot_data": self.fill_slot_data(),
            "location_to_item": {self.location_name_to_id[i.name] : item_table[i.item.name] for i in self.multiworld.get_locations()},
            "data_package": {
                "data": {
                    "games": {
                        self.game: {
                            "item_name_to_id": self.item_name_to_id,
                            "location_name_to_id": self.location_name_to_id
                        }
                    }
                }
            }
        }
        filename = f"{self.multiworld.get_out_file_name_base(self.player)}.appvzf"
        with open(os.path.join(output_directory, filename), 'w') as f:
            json.dump(data, f)

    def extend_hint_information(self, hint_data: typing.Dict[int, typing.Dict[int, str]]):
        return

    def write_spoiler(self, spoiler_handle: typing.TextIO) -> None:
        # Write calculated star costs to spoiler.
        star_cost_spoiler_header = '\n\n' + self.player_name + ' line 159, TODO find out what this does:\n\n'
        spoiler_handle.write(self.player_name)
        # - Reformat star costs dictionary in spoiler to be a bit more readable.
