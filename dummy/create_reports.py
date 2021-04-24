import datetime
import random
import json
import os
from collections import Counter

from faker import Faker

numOfVotings = 10
numOfOptions = 10
def get_all_votings():
    with open(os.getcwd()+'/data/votings.json') as f:
        data = json.load(f)
    return data  
def getWinner(data):
   
    higher = (-1,0)
    for i in range(numOfVotings):
        for j in range(numOfOptions):
            if  int(data['options'][j]['voteCount']) > higher[1]:
                higher =(j,int(data['options'][j]['voteCount']))
    return higher[0]
def gen_report(data):
    
    if data['active']:
        winner = data['options'][getWinner(data)]
    else:
        winner = 'null'
    report= {
        'winner': winner ,
        'participantCount': sumVotes(data)
    }
    
    return report
def sumVotes(data):
    summedVotes = 0
    
    for j in range(numOfOptions):
        summedVotes += int(data['options'][j]['voteCount'])
    return summedVotes

with open("./data/votings.json", "r+") as jsonFile:
    data = json.load(jsonFile)
    for i in range(numOfVotings):
        report=gen_report(data[i])
        data[i]['report'] = report
    jsonFile.seek(0)  # rewind
    json.dump(data, jsonFile, indent = 4)
    jsonFile.truncate()