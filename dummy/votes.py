import datetime
import random
import os
from random_object_id import generate
import json
from faker import Faker

faker = Faker()

optionNumber = 10
# change for other value of voters(now 1000)
def votesCount(x):
    return {
        0: 1000,
        1: 500,
        2: 500,
        3: 334,
        4: 333,
        5: 333,
        6: 250,
        7: 250,
        8: 250,
        9: 250,
    }[x]

def gen_votes(total):
    votes = []
    for i in range(total):
        
        vote= {
            'voteId': generate(),
            'votingOptionNumber': faker.random_int(0, 9)
        }
        votes.append(vote)
    return votes


counter = 1
for i in range(optionNumber):
    path = os.getcwd()+"/data/votes_data/"
    if not os.path.exists(path):
        os.makedirs(path)
    total = votesCount(i)
    votes = gen_votes(total)
    with open(path+str(counter)+'_votes_data.json', 'w') as json_file:
        json.dump(votes, json_file, indent = 4)
        json_file.close()
    counter += 1
