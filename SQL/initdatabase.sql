INSERT INTO dbo.Roles ( Role_Name) VALUES
( 'Admin'),
( 'Gestion Ecole'),
( 'Professeur'),
( 'Eleve')


INSERT INTO Users (User_email, User_lastname, User_firstname, User_RoleRoles_Id, User_School_Id, User_password)
VALUES ('test1', 'Debras', 'Nicolas', 1, 1, 'test')
, ('test2', 'Debras', 'Robert', 1, 1, 'test') 
, ('test3', 'Debras', 'Bastien', 2, 1, 'test')
, ('test4', 'Debras', 'lucas', 2, 1, 'test')
, ('test5', 'Debras', 'Jean', 3, 1, 'test')
, ('test6', 'Debras', 'yoyo', 3, 1, 'test')
, ('test7', 'Debras', 'Jean-Eud', 4, 1, 'test')
, ('test8', 'Debras', 'yoyol', 4, 1, 'test')


ALTER TABLE Users
ALTER COLUMN [User_email] nvarchar(255) NOT NULL;

ALTER TABLE Users
ADD CONSTRAINT unique_user_email UNIQUE (User_email);

ALTER TABLE Schools
ALTER COLUMN [School_Name] nvarchar(255) NOT NULL;

ALTER TABLE Schools
ADD CONSTRAINT unique_School_Name UNIQUE (School_Name);



