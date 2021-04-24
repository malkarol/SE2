import datetime
import random
import json
import os 

from datetime import date
from faker import Faker
from random_object_id import generate

faker = Faker()

start_date = datetime.date(2020, 1, 1)
end_date = date.today()

time_between_dates = end_date - start_date
days_between_dates = time_between_dates.days

def getVotesData():
    listOfLists = []
    for i in range(10):
        with open(os.getcwd()+'/data/vote_references_data/'+str(i+1)+'_vote_references_data.json') as f:
            data = json.load(f)
        listOfLists.append(data)    
    return listOfLists  



def getEligibleVotings(iter, voters,data):
    listOfLists = []
    listOfLists.append(data[0][0]['votingId'])

    quarter = voters/4 
    
    if iter%2==0:
        listOfLists.append(data[1][0]['votingId'])
    else:
        listOfLists.append(data[2][0]['votingId'])
    if iter%3==0:
        listOfLists.append(data[3][0]['votingId'])
    elif iter%3==1:
        listOfLists.append(data[4][0]['votingId'])
    else:
        listOfLists.append(data[5][0]['votingId'])
    if iter < quarter:
        listOfLists.append(data[6][0]['votingId'])
    elif iter >= quarter and  iter < 2* quarter:
        listOfLists.append(data[7][0]['votingId'])
    elif iter >= 2 * quarter and  iter < 3* quarter:
        listOfLists.append(data[8][0]['votingId'])
    else:
        listOfLists.append(data[9][0]['votingId'])
    return listOfLists

def getVotingsReferences(iter, voters,data):
    listOfLists = []
    listOfLists.append(data[0].pop())

    quarter = voters/4 
    
    if iter%2==0:
        listOfLists.append(data[1].pop())
    else:
        listOfLists.append(data[2].pop())
    if iter%3==0:
        listOfLists.append(data[3].pop())
    elif iter%3==1:
        listOfLists.append(data[4].pop())
    else:
        listOfLists.append(data[5].pop())
    if iter < quarter:
        listOfLists.append(data[6].pop())
    elif iter >= quarter and  iter < 2* quarter:
        listOfLists.append(data[7].pop())
    elif iter >= 2 * quarter and  iter < 3* quarter:
        listOfLists.append(data[8].pop())
    else:
        listOfLists.append(data[9].pop())
    return listOfLists

def gen_voter(total):
    voters = []
    data_votes = getVotesData()

    
    for i in range(total):
        fn=faker.first_name()
        ln=faker.last_name()
        random_number_of_days = random.randrange(days_between_dates)
        random_date = start_date + datetime.timedelta(days=random_number_of_days)
        
        voter= {
            "id": generate(),
            "firstName": fn,
            "lastName": ln,
            "email": faker.unique.email(fn),
            "password": faker.password(),
            "registrationDate": random_date.strftime('%Y-%m-%dT%H:%M:%S'),
            "eligibleVotingsIds": [
                getEligibleVotings(i,total,data_votes)
            ],
            "voteReferences": [
                getVotingsReferences(i,total,data_votes)
            ]
        }
          
        voters.append(voter)
    return voters

vs = gen_voter(1000)

with open('./data/voters.json', 'w') as json_file:
  json.dump(vs, json_file, indent = 4)