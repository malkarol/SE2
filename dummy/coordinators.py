from faker import Faker
from random_object_id import generate
import json
import os 
faker = Faker()

def get_all_votings():
    with open(os.getcwd()+'/votings.json') as f:
        data = json.load(f)
    return data

def assingCoordinator(iter, data):
    listOfLists = []
    if iter < 5:
        listOfLists.append(data[2*iter]['id'])
        listOfLists.append(data[2*iter+1]['id'])
    else:
        listOfLists.append(data[iter-5]['id'])
    return listOfLists

def assingNormalCoordinator(iter, data):
    listOfLists = []
    listOfLists.append(data[2*iter]['id'])
    listOfLists.append(data[2*iter+1]['id'])
    return listOfLists

def gen_coordinator(total):
    coordinators = []
    data = get_all_votings()
    types = ['Normal','Main']
    for i in range(total):
        fn = faker.unique.first_name()
        ln = faker.unique.last_name()
        
        coordinator= {
            'id': generate(),
            'first_name': fn,
            'last_name': ln,
            'coordinate_type': types[1] if i < 5 else types[0],
            'email': faker.email(fn),
            'password': faker.password(),
            'votingsIds': assingCoordinator(i,data)

        }
        coordinators.append(coordinator)
    return coordinators

coordinators = gen_coordinator(15)

with open('coordinators.json', 'w') as json_file:
  json.dump(coordinators, json_file, indent = 4)
  json_file.close()
