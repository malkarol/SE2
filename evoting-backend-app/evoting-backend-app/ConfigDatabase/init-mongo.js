// Creates new entry in admin/system.users
db.createUser(
    {
        user: "evotingAdmin",
        pwd: "123",
        roles: [
            {
                role: "readWrite",
                db: "evoting"
            }
        ]
    }
);

// --- Create database, collections, and documents ---

// - Main database -
db = db.getSiblingDB('evotingMain'); 

db.createCollection("voters");
//db.voters.insert([
//    { _id: ObjectId("606b498d982ca2c8da77e632"), FirstName: "John", LastName: "Woznicki", Email: "J.Woznicki@mail.com", Password: "myPassword123" },
//    { _id: ObjectId("606b498d982ca2c8da77e633"), FirstName: "Adam", LastName: "Lambert", Email: "Adam.L@mail.com", Password: "al123abc" }
//    { _id: ObjectId("606b498d982ca2c8da77e634"), FirstName: "Thomas", LastName: "Knight", Email: "T.Kn@mail.com", Password: "dwadawwc" }
//    { _id: ObjectId("606b498d982ca2c8da77e635"), FirstName: "Eva", LastName: "Right", Email: "Eva.R@mail.com", Password: "fafwqtv" }
//]);

db.createCollection("coordinators");

db.createCollection("votings");

// - Votes database -
db = db.getSiblingDB("evotingVotes")

db.createCollection("voting_test1");

db.createCollection("voting_test2");

// - Registration Requests database -

db = db.getSiblingDB('evotingRegistrationRequests');

db.createCollection("voting_test1");

db.createCollection("voting_test2");