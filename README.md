# Aditum
Granular User Access Management - Users, Groups, Operations, Permissions (fully thread-safe)



| | |
| --- | --- |
| **Quality** | [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=raminrahimzada_SQLEngine&metric=alert_status)](https://sonarcloud.io/dashboard?id=Aditum.Core) [![GitHub issues](https://img.shields.io/github/stars/raminrahimzada/Aditum)](https://github.com/raminrahimzada/Aditum/issues) | 
| **Nuget** | [![Nuget](https://buildstats.info/nuget/Aditum.Core)](http://nuget.org/packages/Aditum.Core) |


With Aditum You can set permission to user or group according to Operations<br/>
User , Group and Operation details are not stored in Aditum <br/>
You can only set it's identity - Id ,<br/>
If you want to store additional detail -  just store them elsewhere with referencing it's id  <br/>
Because Aditum is intended only for authorization purposes <br/>
Here Id can be int,long,Guid and whatever you want 

# Configuration  (for asp.net core configuration see [this](https://github.com/raminrahimzada/Aditum/tree/master/DemoAspNetCoreApp))
For example if our user id,group id, operation id is int and our permission is just a bool - yes/no 
then
```cs
public class AppUserService : UserService<int,int,int,bool>
{
...
```

If we want to mention that we have these users,groups,operations:
```cs
//getting instance 
var service = new AppUserService();

//just numbering their id's

//user
const int bob = 1;
const int tom = 2;
//groups
const int admins = 1;
//operations
const int canSeeSecretsOfUniverse = 1;
const int canChangeSecretsOfUniverse = 2;


//mentioning that we have these
service.EnsureUserId(bob);
service.EnsureGroupId(admins);
service.EnsureOperationId(canSeeSecretsOfUniverse);

//change group permissions
service.SetGroupPermission(admins, canSeeSecretsOfUniverse, true);
service.SetGroupPermission(admins, canChangeSecretsOfUniverse, true);

//add tom and bob to admins
service.EnsureUserIsInGroup(bob, admins);
service.EnsureUserIsInGroup(tom, admins);


//deny bob for canChangeSecretsOfUniverse although he is admin
service.SetUserExclusivePermission(bob,admins,false);

//these will return true and  true
var tomCanSee = GetUserPermission(tom,canSeeSecretsOfUniverse);
var tomCanChange = GetUserPermission(tom,canChangeSecretsOfUniverse);

//these will return true and false
var bobCanSee = GetUserPermission(bob,canSeeSecretsOfUniverse);
var bobCanChange = GetUserPermission(bob,canChangeSecretsOfUniverse);
//Because we exclusively deny bob for canChangeSecretsOfUniverse operation
//although he is in admins and in admins by default all users can 'canChangeSecretsOfUniverse'
```

# Persistence
```cs
//Aditum has thread-safe change mechanism so you can listen for its setting change event 
//to store current settings and load the next time system startup
//For that purpose there is 2 methods

//dumping to file or stream:

service.DumpTo("aditum.db");
//or
service.DumpTo(stream);


//and loading from file or stream :

service.LoadFrom("aditum.db");
//or
service.LoadFrom(stream);
```
Dump/Load is  using memory mapped structure and this causes less space (usually in KBs)  <br/>
To Optimize dump/load speed and storage size customize serialization strategy,<br/>
If you are using int,long,guid or byte as id type then there is built in serialization strategies implemented <br/>
For detailed configuration  code see [this](https://github.com/raminrahimzada/Aditum/blob/master/DemoAspNetCoreApp/AppUserService.cs)

