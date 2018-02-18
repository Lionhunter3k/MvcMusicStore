This is the old Music Store sample project for MVC3, modified to use Castle Windsor for controller/filter dependency injection and nHibernate 3.x for data access concerns. 

Made this a loooooooong time ago in Visual 2010, should run on Visual 2017 IF MVC3 is found in the GAC (you can install from here https://www.microsoft.com/en-us/download/details.aspx?id=1491).

At that time, I was into Oracle databases, so, by default, nHibernate is configured for an Oracle database, but for testing purposes, the nHibernate installer uses a local SQLite database, so you can pretty much just F5 the project.