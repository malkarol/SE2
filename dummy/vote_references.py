import datetime
import random
from random_object_id import generate
import os 
import json
from faker import Faker

faker = Faker()

numOfFiles = 10
def get_all_votings():
    with open(os.getcwd()+'/data/votings.json') as f:
        data = json.load(f)
    return data  
def getVotesData(iter):
    with open(os.getcwd()+'/data/votes_data/'+str(iter+1)+'_votes_data.json') as f:
        data = json.load(f)
    return data   

def assingVotings(iter, voters,data):
    listOfLists = []
    listOfLists.append(data[0]['id'])
    quarter = voters/4 
    
    if iter%2==0:
        listOfLists.append(data[1]['id'])
    else:
        listOfLists.append(data[2]['id'])
    if iter%3==0:
        listOfLists.append(data[3]['id'])
    elif iter%3==1:
        listOfLists.append(data[4]['id'])
    else:
        listOfLists.append(data[5]['id'])
    if iter < quarter:
        listOfLists.append(data[6]['id'])
    elif iter >= quarter and  iter < 2* quarter:
        listOfLists.append(data[7]['id'])
    elif iter >= 2 * quarter and  iter < 3* quarter:
        listOfLists.append(data[8]['id'])
    else:
        listOfLists.append(data[9]['id'])
    return listOfLists

def gen_vote_reference(total, iter, votesIds, thisVotingId):
    votes = []
    votingId = thisVotingId
    for i in range(total):
        votingsIDs = assingVotings
        vote= {
            'votingId': votingId,
            'voteId': votesIds[i]['voteId'],
        }
        votes.append(vote)
    return votes
votingReferences = []
votings = get_all_votings() 
counter=1
for i in range(10):
    
    votes = getVotesData(i)
    total = len(votes)
    votingId = votings[i]['id']
    v = gen_vote_reference(total, i, votes, votingId)
    path = os.getcwd()+"/data/vote_references_data/"
    if not os.path.exists(path):
        os.makedirs(path)
    with open(path+str(counter)+'_vote_references_data.json', 'w') as json_file:
        json.dump(v, json_file, indent = 4)
        json_file.close()
    counter += 1

