
    drop table OrderDetail cascade constraints

    drop table AnonymousUser cascade constraints

    drop table RegisteredUser cascade constraints

    drop table Genre cascade constraints

    drop table PurchaseOrders cascade constraints

    drop table Album cascade constraints

    drop table Artist cascade constraints

    drop table CartItem cascade constraints

    drop table hibernate_unique_key cascade constraints

    create table OrderDetail (
        Id NUMBER(10,0) not null,
       Quantity NUMBER(10,0) not null,
       UnitPrice NUMBER(19,5) not null,
       Album NUMBER(10,0),
       PartOfOrder NUMBER(10,0),
       OrderId NUMBER(10,0),
       AlbumId NUMBER(10,0),
       primary key (Id)
    )

    create table AnonymousUser (
        Id NUMBER(10,0) not null,
       LatestAddress NVARCHAR2(255) not null,
       Role NVARCHAR2(255) not null,
       primary key (Id)
    )

    create table RegisteredUser (
        Id NUMBER(10,0) not null,
       Username NVARCHAR2(255) not null,
       Password NVARCHAR2(255) not null,
       Question NVARCHAR2(255),
       Answer NVARCHAR2(255),
       Email NVARCHAR2(255) not null,
       primary key (Id)
    )

    create table Genre (
        Id NUMBER(10,0) not null,
       Name NVARCHAR2(255) not null,
       Description NVARCHAR2(255),
       primary key (Id)
    )

    create table PurchaseOrders (
        Id NUMBER(10,0) not null,
       OrderDate TIMESTAMP(4) not null,
       Username NVARCHAR2(255) not null,
       FirstName NVARCHAR2(255) not null,
       LastName NVARCHAR2(255) not null,
       Address NVARCHAR2(255) not null,
       City NVARCHAR2(255) not null,
       State NVARCHAR2(255) not null,
       PostalCode NVARCHAR2(255) not null,
       Country NVARCHAR2(255) not null,
       Phone NVARCHAR2(255) not null,
       Email NVARCHAR2(255) not null,
       Total NUMBER(19,5) not null,
       UserId NUMBER(10,0),
       primary key (Id)
    )

    create table Album (
        Id NUMBER(10,0) not null,
       Title NVARCHAR2(255) not null,
       Price NUMBER(19,5) not null,
       AlbumArtUrl NVARCHAR2(255) not null,
       GenreId NUMBER(10,0),
       ArtistId NUMBER(10,0),
       primary key (Id)
    )

    create table Artist (
        Id NUMBER(10,0) not null,
       Name NVARCHAR2(255) not null,
       primary key (Id)
    )

    create table CartItem (
        Id NUMBER(10,0) not null,
       Count NUMBER(10,0) not null,
       Album NUMBER(10,0),
       UserId NUMBER(10,0) not null unique,
       primary key (Id)
    )

    alter table OrderDetail 
        add constraint FK2F183D34F29C3C72 
        foreign key (Album) 
        references Album

    alter table OrderDetail 
        add constraint FK2F183D3455C84549 
        foreign key (PartOfOrder) 
        references PurchaseOrders

    alter table OrderDetail 
        add constraint FK2F183D348F0B119F 
        foreign key (OrderId) 
        references PurchaseOrders

    alter table OrderDetail 
        add constraint FK2F183D34A55A0654 
        foreign key (AlbumId) 
        references Album

    alter table RegisteredUser 
        add constraint FK34E1D74B29063CF3 
        foreign key (Id) 
        references AnonymousUser

    create index GENRENAME_INDEX on Genre (Name)

    alter table PurchaseOrders 
        add constraint FK923BF07EB4E28F5E 
        foreign key (UserId) 
        references RegisteredUser

    create index TITLE_INDEX on Album (Title)

    alter table Album 
        add constraint FKA0CE20AAD28AA89C 
        foreign key (GenreId) 
        references Genre

    alter table Album 
        add constraint FKA0CE20AAA91C26CE 
        foreign key (ArtistId) 
        references Artist

    create index NAME_INDEX on Artist (Name)

    alter table CartItem 
        add constraint FKB3204199F29C3C72 
        foreign key (Album) 
        references Album

    alter table CartItem 
        add constraint FKB3204199CF20F0DF 
        foreign key (UserId) 
        references AnonymousUser

    create table hibernate_unique_key (
         next_hi NUMBER(10,0) 
    )

    insert into hibernate_unique_key values ( 1 )
