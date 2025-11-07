
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

DROP TABLE IF EXISTS "TimeEntry";


-- extensions
CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;


--
-- stores app user accounts
--

CREATE TABLE IF NOT EXISTS "Users" (
  "UserId" uuid PRIMARY KEY DEFAULT gen_random_uuid(),  -- internal id
  "Email" citext NOT NULL UNIQUE,                       -- case insensitive login name
  "PasswordHash" text NOT NULL,                         -- bcrypt
  "DisplayName" text,
  "IsActive"     boolean NOT NULL DEFAULT TRUE,        -- kill switch
  "CreatedAt"    timestamptz NOT NULL DEFAULT now(),
  "UpdatedAt"    timestamptz NOT NULL DEFAULT now()
);

--
-- used for session management (JWT refresh support)
--

CREATE TABLE IF NOT EXISTS "RefreshTokens" (
  "RefreshTokenId"  uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "UserId"           uuid NOT NULL REFERENCES "Users"("UserId") ON DELETE CASCADE,
  "Token"             text NOT NULL UNIQUE,          -- random opaque string (not a JWT)
  "CreatedAt"        timestamptz NOT NULL DEFAULT now(),
  "ExpiresAt"        timestamptz NOT NULL,
  "RevokedAt"       timestamptz,
  "ReplacedByToken" text                           -- rotation chain tracking (optional)
);

-- index lookup
CREATE INDEX IF NOT EXISTS "idx_refresh_tokens_user_id" ON "RefreshTokens"("UserId");

--
-- tracks user clock ins and outs
--

CREATE TABLE IF NOT EXISTS "TimeEntry" (
  "TimeEntryId" BIGSERIAL PRIMARY KEY,
  "UserId" uuid NOT NULL,
  "AssignmentId" INT NULL,
  "StartTime" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "EndTime" TIMESTAMPTZ NULL,

  CONSTRAINT fk_timeentry_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE,
  CONSTRAINT chk_timeentry_times CHECK ("EndTime" IS NULL OR "EndTime" >= "StartTime"),
  CONSTRAINT fk_timeentry_assignment FOREIGN KEY ("AssignmentId") REFERENCES "Assignment"("AssignmentId")
);

DROP TABLE IF EXISTS "RequestOff";

CREATE TABLE IF NOT EXISTS "RequestOff" (
  "RequestOffId" BIGSERIAL PRIMARY KEY,
  "UserId"       UUID NOT NULL,
  "StartDate"    DATE NOT NULL,
  "EndDate"      DATE NOT NULL,
  "Note"         VARCHAR,
  "CreatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now(),
  "UpdatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now(),

  CONSTRAINT fk_requestoff_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE,
  CONSTRAINT chk_requestoff_dates CHECK ("StartDate" <= "EndDate")
);

--
-- represents jobsites that employees are assigned to
--

CREATE TABLE IF NOT EXISTS "Assignment" ( -- to do
  "AssignmentId" SERIAL PRIMARY KEY, -- unique id per jobsite
  "TotalHours" INT NOT NULL DEFAULT 0, -- stored hours at that jobsite
  "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(), -- creation of a jobsite
  "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(), -- last modified, dk if this is even neccessary but its here

  CONSTRAINT chk_assignment_hours_nonneg CHECK ("TotalHours" >= 0) -- making sure no negative hours
);

--
-- links which users are assigned to which jobsites
--

CREATE TABLE IF NOT EXISTS "UserAssignment" (
  "AssignmentId" INT NOT NULL, -- foreign key to Assignment
  "UserId" UUID NOT NULL, -- foreign key to Users

  PRIMARY KEY ("AssignmentId", "UserId"), -- each pair is unique

  CONSTRAINT fk_userassignment_assignment FOREIGN KEY ("AssignmentId") REFERENCES "Assignment"("AssignmentId") ON DELETE CASCADE, -- deletes remove join rows if jobsite or user removed
  CONSTRAINT fk_userassignment_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE -- 
);

-- fast lookup indexes by user or assignment
CREATE INDEX IF NOT EXISTS idx_userassignment_user       ON "UserAssignment"("UserId");
CREATE INDEX IF NOT EXISTS idx_userassignment_assignment ON "UserAssignment"("AssignmentId");

--
-- tracks individual tasks for users, linked to Assignment
--

-- status enum for workflow stages
CREATE TYPE IF NOT EXISTS todo_status AS ENUM ('todo', 'in_progress', 'done');

CREATE TABLE IF NOT EXISTS "ToDoItem" (
  "ToDoId" BIGSERIAL PRIMARY KEY, -- unique task Id
  "UserId" UUID NOT NULL, -- user responsible for the task (idk maybe)
  "AssignmentId" INT NULL -- linking to a jobsite (assignment)
  "Title" VARCHAR NOT NULL, -- task name
  "Details" TEXT,
  "Status" todo_status NOT NULL DEFAULT 'todo', -- current stage = todo, in_progress, done
  "DueAt" TIMESTAMPTZ NULL,
  "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),

  -- linking to user assigned to this task
  CONSTRAINT fk_todo_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE,

  -- linking to jobsite, if deleted keep todo but unlink
  CONSTRAINT fk_todo_assignment FOREIGN KEY ("AssignmentId") REFERENCES "Assignment"("AssignmentId") ON DELETE SET NULL,

  -- link to a specific time entry for tasks tied to a session, if entry is deleted unlink but keep todo
  CONSTRAINT fk_todo_timeentry FOREIGN KEY ("TimeEntryId") REFERENCES "TimeEntyry"("TimeEntryId") ON DELETE SET NULL -- didnt put much thought into this, might not even want it

  -- preventing blank titles
  CONSTRAINT chk_todo_title_not_blank CHECK (length(trim("Title")) > 0)
);

-- lookup tasks by user
CREATE INDEX IF NOT EXISTS idx_todo_user        ON "TodoItem"("UserId");

-- lookup by assignment/jobsite
CREATE INDEX IF NOT EXISTS idx_todo_assignment  ON "TodoItem"("AssignmentId");

-- lookup by linked time entry
CREATE INDEX IF NOT EXISTS idx_todo_timeentry   ON "TodoItem"("TimeEntryId");

-- filter by status
CREATE INDEX IF NOT EXISTS idx_todo_status      ON "TodoItem"("Status");

-- sort/filter by due date
CREATE INDEX IF NOT EXISTS idx_todo_due         ON "TodoItem"("DueAt");