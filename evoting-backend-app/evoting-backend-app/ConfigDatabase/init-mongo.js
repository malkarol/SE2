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

// Create database, collections, and documents
db = db.getSiblingDB('evoting'); 

db.createCollection("users");
db.users.insert([
    { _id: "606b498d982ca2c8da77e632", FirstName: "John", LastName: "Woznicki", Email: "J.Woznicki@mail.com", Password: "myPassword123" },
    { _id: "606b498d982ca2c8da77e633", FirstName: "Adam", LastName: "Lambert", Email: "Adam.L@mail.com", Password: "al123abc" }
]);

db.createCollection("coordinators");

db.createCollection("votings");

db.createCollection("registrationRequests");

db.createCollection("votes"); //?
