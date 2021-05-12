import datetime
import random
import json
import os
from collections import Counter

from faker import Faker

faker = Faker()
votingOptions = {}
maxNumOfVotingOptions = 10
votings = ['MazovianElection2020', 'SilesianElection2020', 'PomeranianElection2020', 'BestRestaurantWarsaw2020', 'BestFootbalPlayer2020',
           'BestPolishBook2020', 'UniversityRanking2020', 'UniversityRanking2021', 'BestColumbianCoffee2020', 'CityBudget2020']
coffee = ['Brazzzzilian', 'BlackCATCoffee', 'Ethiopian', 'south_africa_express', 'MuchoGusto']
restaurants = ['RamenPlace','ThaiCousinePlace','RussianCousinePlace','SushiPlace','FastFoodPlace']
univerisities = ['BlaBla University of Technology','University of SomePlaceNice',
'Mhmmm Insititute of Technology','Kaer Morhen',' Jedi Temple']

def getVoteCounts():
    with open(os.getcwd()+'/data/counting_votes.json') as f:
            data = json.load(f)  
    return data 

def gen_default_votingOptions(lista):
    votingOptions = []
    k = 0
    while(k < maxNumOfVotingOptions):
        randomName = lista[random.randrange(len(lista))]
        numberOfOccurence = 0
        for elem in votingOptions:
            if randomName in elem:
                numberOfOccurence += 1 
        if  numberOfOccurence > 0:
            
            votingOptions.append(randomName+" "+str(numberOfOccurence+1))
        else:
            votingOptions.append(randomName)
        k = k + 1
    return votingOptions

def gen_random_votingOptions(number):
    votingOptions = []
    k = 0
    if(number == 1):
        while(k < maxNumOfVotingOptions):
            randomName = faker.first_name()+" "+faker.last_name()
            while(randomName in votingOptions):
                randomName = faker.first_name()+" "+faker.last_name()
            votingOptions.append(randomName)
            k = k + 1
    else:
        while(k < maxNumOfVotingOptions):
            randomName = ' '.join(faker.words(faker.random_int(1, 10)))
            votingOptions.append(randomName)
            k = k + 1
    return votingOptions





votingOptions['MazovianElection2020'] = gen_random_votingOptions(1)
votingOptions['SilesianElection2020'] = gen_random_votingOptions(1)
votingOptions['PomeranianElection2020'] = gen_random_votingOptions(1)
votingOptions['BestFootbalPlayer2020'] = gen_random_votingOptions(1)
votingOptions['CityBudget2020'] = gen_random_votingOptions(0)
votingOptions['BestPolishBook2020'] = gen_random_votingOptions(0)

votingOptions['BestRestaurantWarsaw2020'] = gen_default_votingOptions(restaurants)
votingOptions['UniversityRanking2020'] = gen_default_votingOptions(univerisities)
votingOptions['UniversityRanking2021'] = gen_default_votingOptions(univerisities)
votingOptions['BestColumbianCoffee2020'] = gen_default_votingOptions(coffee)


def gen_voting_option(total, votingName):

    votings = []
    names = votingOptions[votingName]

    random_ids = random.sample(range(0, total), total)
    for i in range(total):
        random_id = random_ids[i]
        voting= {
            'number': i,
            'name': names[i],
            'description': faker.text(),
            'voteCount'	: 'null' ,
        }
        votings.append(voting)
    return votings

counter = 1
results = {}
for voting in votingOptions:
    vt = gen_voting_option(maxNumOfVotingOptions, voting)
    results[voting] = vt

for voting in results:  
    path = os.getcwd()+"/data/voting_option_data/"
    if not os.path.exists(path):
        os.makedirs(path)

    with open(path+voting+'_voting_options.json', 'w') as json_file:
        json.dump(results[voting], json_file, indent = 4)
        json_file.close()
    counter+=1
