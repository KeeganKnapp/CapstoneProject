
-- postgresSQL does not have a 'use' command to select the database, you have to select the database when connecting
-- CREATE DATABASE capstone;

DROP TABLE IF EXISTS userRecipient;
DROP TABLE IF EXISTS notification;
DROP TABLE IF EXISTS userRequest;
DROP TABLE IF EXISTS request;
DROP TABLE IF EXISTS userAssignment;
DROP TABLE IF EXISTS assignment;
DROP TABLE IF EXISTS employee;
DROP TYPE role;
DROP TYPE requestType;
DROP TYPE messageType;

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

CREATE TABLE IF NOT EXISTS "Users" (
  "UserId" uuid PRIMARY KEY DEFAULT gen_random_uuid(),  -- internal id
  "Email" citext NOT NULL UNIQUE,                       -- case insensitive login name
  "PasswordHash" text NOT NULL,                         -- bcrypt
  "DisplayName" text,
  "IsActive"     boolean NOT NULL DEFAULT TRUE,        -- kill switch
  "CreatedAt"    timestamptz NOT NULL DEFAULT now(),
  "UpdatedAt"    timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS "RefreshTokens" (
  "RefreshTokenId"  uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "UserId"           uuid NOT NULL REFERENCES "Users"("UserId") ON DELETE CASCADE,
  "Token"             text NOT NULL UNIQUE,          -- random opaque string (not a JWT)
  "CreatedAt"        timestamptz NOT NULL DEFAULT now(),
  "ExpiresAt"        timestamptz NOT NULL,
  "RevokedAt"       timestamptz,
  "ReplacedByToken" text                           -- rotation chain tracking (optional)
);

CREATE INDEX IF NOT EXISTS "idx_refresh_tokens_user_id" ON "RefreshTokens"("UserId");

-- Switched this to an ENUM in case we ever expand this to something like owner, manager, employee etc.
-- If we change this you drop an ENUM via the command below, otherwise it will fail
-- DROP TYPE enumNameHere;
CREATE TYPE role AS ENUM ('management', 'employee');
CREATE TABLE employee (
  employeeId serial NOT NULL,
  username varchar NOT NULL UNIQUE,
  password varchar NOT NULL,
  firstName varchar NOT NULL,
  -- Controls user's role
  management role NOT NULL,
  PRIMARY KEY(employeeId)
);

-- examples so I don't need to keep writing this out

-- additionally, serial still increments even on a failed insert (e.g. first insert succeeds, second fails, third succeeds. Despite there only being 2 entries the entries are 1 and 3. 2 has been skipped.)
-- you can ignore serial when inserting. Serial additionally is just an integer, it has the same limit.
-- insert into employee (username,password,firstName,management) values ('test','test','test','employee');

-- or you can use DEFAULT
-- insert into employee (employeeID, username,password,firstName,management) values (DEFAULT, 'test2','test2','test2','management');

CREATE TABLE assignment (
  assignmentId serial NOT NULL,
  -- I've chosen point to store the location of the job site as it should suit our needs
  -- If we need something more advanced the extention PostGIS specializes in storing and working with geospatial data
  site point NOT NULL,
  -- Used to calculate an employee's hours
  totalHours int NOT NULL,
  -- Whether the job is archived/finished or not
  archived bool NOT NULL,
  PRIMARY KEY(assignmentId)
);

CREATE TABLE userAssignment (
  assignmentId int NOT NULL,
  employeeId int NOT NULL,
  PRIMARY KEY(assignmentId, employeeId),
  FOREIGN KEY (assignmentId) REFERENCES assignment(assignmentId) ON DELETE CASCADE,
  FOREIGN KEY (employeeId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

CREATE TYPE requestType AS ENUM ('extention', 'timeOff');
CREATE TABLE request (
	requestId serial NOT NULL,
	reqType requestType NOT NULL,
	requestTimeStart date,
	requestTimeEnd date,
	requestNote varchar,
  PRIMARY KEY(requestId)
);

CREATE TABLE userRequest (
  requestId int NOT NULL,
  employeeId int NOT NULL,
  PRIMARY KEY(requestId, employeeId),
  FOREIGN KEY (requestId) REFERENCES request(requestId) ON DELETE CASCADE,
  FOREIGN KEY (employeeId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

-- These are just examples, feel free to add more
-- Make sure to drop the enum first via query above
CREATE TYPE messageType AS ENUM ('extention', 'timeOff', 'report', 'problem');
CREATE TABLE notification (
  notificationId serial NOT NULL,
  senderId int NOT NULL,
  notifType messageType NOT NULL,
  notifContents varchar NOT NULL,
  PRIMARY KEY(notificationId),
  FOREIGN KEY (senderId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

CREATE TABLE userRecipient (
  notificationId int NOT NULL,
  recipientId int NOT NULL,
  PRIMARY KEY(notificationId,recipientId),
  FOREIGN KEY (notificationId) REFERENCES notification(notificationId) ON DELETE CASCADE,
  FOREIGN KEY (recipientId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "TimeEntry" (
  "TimeEntryId" BIGSERIAL PRIMARY KEY,
  "EmployeeId" INT NOT NULL,
  "AssignmentId" INT NULL,
  "StartTime" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "EndTime" TIMESTAMPTZ NULL
);