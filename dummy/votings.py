import datetime
import random
import json
import os
from faker import Faker
from random_object_id import generate

faker = Faker()

start_date = datetime.date(2020, 1, 1)
end_date = datetime.date(2021, 12, 31)

time_between_dates = end_date - start_date
days_between_dates = time_between_dates.days
names=['MazovianElection2020','SilesianElection2020','PomeranianElection2020','BestRestaurantWarsaw2020','BestFootbalPlayer2020',
        'BestPolishBook2020','UniversityRanking2020','UniversityRanking2021','BestColumbianCoffee2020','CityBudget2020']

def isActive(starting_date, ending_date):
    today = datetime.datetime.now()
    start = datetime.datetime.strptime(starting_date, '%Y-%m-%dT%H:%M:%S')
    end = datetime.datetime.strptime(ending_date, '%Y-%m-%dT%H:%M:%S')
    return True if today >= start and today <= end else False

def getWinner(is_active):
    if is_active:
        return "null"
    # get votingOption with most Votes 
    # highest Vote Count
votingOptions = {}

def get_all_votingOptions(voting):
    with open(os.getcwd()+'/data/voting_option_data/'+voting+'_voting_options.json') as f:
        data = json.load(f)
    return data

def gen_voting(total):
    votings = []
    for i in range(total):
        
        random_number_of_days = random.randrange(days_between_dates)
        random_date = start_date + datetime.timedelta(days=random_number_of_days)
        random_end_date = end_date + datetime.timedelta(days=random_number_of_days)- datetime.timedelta(days=random.randrange(days_between_dates))
        random_id = faker.unique.random_int(1, len(names))
        local_start_date = random_date.strftime('%Y-%m-%dT%H:%M:%S')
        local_end_date = random_end_date.strftime('%Y-%m-%dT%H:%M:%S')
        voting= {
            'id': generate(),
            'name': names[random_id-1],
            'description': faker.text(),
            'start_date': local_start_date,
            'end_date': local_end_date,
            'active': isActive(local_start_date, local_end_date),
            'options': get_all_votingOptions(names[random_id-1]),
            'report': 'null'
        }
        votings.append(voting)
    return votings

vt = gen_voting(10)


with open(os.getcwd()+'/data/'+'votings.json', 'w') as json_file:
  json.dump(vt, json_file, indent = 4)
  json_file.close()
