import datetime
import random
import json
import os
from collections import Counter

from faker import Faker

numOfVotings = 10
numOfOptions = 10
def getVotesData(iter):
    with open(os.getcwd()+'/data/votes_data/'+str(iter+1)+'_votes_data.json') as f:
        data = json.load(f)
    return data 
finalResult =  []
for i in range(numOfVotings):
    votes = getVotesData(i)
    result = {}
    for j in range(numOfOptions):
        result[j]=0
    for vote in votes:
        optionNum = vote['votingOptionNumber']
        tmp = result[optionNum]
        tmp +=1
        result[optionNum]=tmp
    finalResult.append(result)

with open("./data/votings.json", "r+") as jsonFile:
    data = json.load(jsonFile)
    for i in range(numOfVotings):
        for j in range(numOfOptions):
            data[i]['options'][j]['voteCount']=finalResult[i][j]
    jsonFile.seek(0)  # rewind
    json.dump(data, jsonFile, indent = 4)
    jsonFile.truncate()
