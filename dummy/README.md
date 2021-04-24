# Software Engineering 2
## Project: E-Voting System
### Populate data: 
Run run_scripts.sh file with your "Python path" and enjoy creation of json files with not so dummy data.


### Files
1. coordinators.py : creates 5 Main Coordinators that handle 2 Votings and 10 Normal Coordinators to handle 1 Voting each
2. count_votings.py : updates voteCount field in votings.json.
3. create_reports.py : updates Voting with winnner if Voting is not active(deadline passed).
4. vote_references.py : creates VoteReferences for existing Votings and Votes in vote_references_data.
5. voters.py : creates 1000 Voters, with pseudo-random access to 4 diffrent Votings. 
6. votes.py : creates random, uniformly distributed Votes for each Voting. 1st gets 1000 votes, 2nd and 3rd 500 each,
 4th 334, 5th and 6th 333 each, 7th up to 10th 250 each. Keeps json files in votes_data.
7. voting_options.py : creates 10 unique VotingOptions for 10 unique Votings.
8. votings.py : creates unique 10 Votings.

### Remarks
1. !!! Remember to change python path in run_scripts.sh !!!
2. For now files in voting_option_data do not posses updated voteCount field but the Voting they are part of has this property updated.
3. Depending on further implementation it might be a good idea to give create_reports.py ability to present report if every eligible Voter ahve already voted.
4. Take into consideration that this is fast solution for populating data sets.