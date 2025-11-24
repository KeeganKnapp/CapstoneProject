DROP TABLE IF EXISTS "TodoItem";
DROP TYPE  IF EXISTS todo_status;

DROP TABLE IF EXISTS "UserAssignment";
DROP TABLE IF EXISTS "TimeEntry";
DROP TABLE IF EXISTS "RequestOff";
DROP TABLE IF EXISTS "RefreshTokens";
DROP TABLE IF EXISTS "Assignment";
DROP TABLE IF EXISTS "Users";


-- extensions
CREATE EXTENSION IF NOT EXISTS citext;


--
-- stores app user accounts
--

CREATE TABLE IF NOT EXISTS "Users" (
  "UserId" SERIAL PRIMARY KEY,  -- internal id
  "Email" citext NOT NULL UNIQUE,                       -- case insensitive login name
  "PasswordHash" text NOT NULL,                         -- bcrypt
  "DisplayName" text,
  "IsActive"     boolean NOT NULL DEFAULT TRUE,        -- kill switch
  "CreatedAt"    timestamptz NOT NULL DEFAULT now(),
  "UpdatedAt"    timestamptz NOT NULL DEFAULT now(),
  "Role" TEXT NOT NULL DEFAULT 'Employee' CHECK ("Role" IN ('Employee', 'Manager'))
);

--
-- used for session management (JWT refresh support)
--

CREATE TABLE IF NOT EXISTS "RefreshTokens" (
  "RefreshTokenId"  BIGSERIAL PRIMARY KEY,
  "UserId"           INT NOT NULL REFERENCES "Users"("UserId") ON DELETE CASCADE,
  "Token"             text NOT NULL UNIQUE,          -- random opaque string (not a JWT)
  "CreatedAt"        timestamptz NOT NULL DEFAULT now(),
  "ExpiresAt"        timestamptz NOT NULL,
  "RevokedAt"       timestamptz,
  "ReplacedByToken" text                           -- rotation chain tracking (optional)
);

-- index lookup
CREATE INDEX IF NOT EXISTS "idx_refresh_tokens_user_id" ON "RefreshTokens"("UserId");

-- jobsite table
CREATE TABLE IF NOT EXISTS "Jobsite" (
  "JobsiteId" SERIAL PRIMARY KEY,
  "Name" TEXT NOT NULL,
  "Latitude" DOUBLE PRECISION NOT NULL,
  "Longitude" DOUBLE PRECISION NOT NULL,
  "RadiusMeters" DOUBLE PRECISION NOT NULL,
  "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT now()
);



--
-- represents jobsites that employees are assigned to
--

CREATE TABLE IF NOT EXISTS "Assignment" ( -- to do
  "AssignmentId" SERIAL PRIMARY KEY, -- unique id per jobsite
  "JobsiteId" INT NOT NULL,
  "Title" TEXT NOT NULL,
  "Description" TEXT,
  "Status" TEXT NOT NULL DEFAULT 'todo',
  "TotalHours" INT NOT NULL DEFAULT 0,
  "CreatedByUserId" INT NOT NULL,

  "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),

  -- ensure hours are never negative
  CONSTRAINT chk_assignment_hours_nonneg CHECK ("TotalHours" >= 0),

  -- status
  CONSTRAINT check_assignment_status CHECK ("Status" IN ('todo', 'in_progress', 'done')),

  -- each assignment belongs to a jobsite
  CONSTRAINT fk_assignment_jobsite FOREIGN KEY ("JobsiteId") REFERENCES "Jobsite"("JobsiteId") ON DELETE CASCADE,

  -- must be a valid user to create an assignment
  CONSTRAINT fk_assignment_creator FOREIGN KEY ("CreatedByUserId") REFERENCES "Users"("UserId") ON DELETE RESTRICT
);

-- fast lookups by jobsite or creator
CREATE INDEX IF NOT EXISTS idx_assignment_jobsite ON "Assignment"("JobsiteId");
CREATE INDEX IF NOT EXISTS idx_assignment_created_by ON "Assignment"("CreatedByUserId");

--
-- tracks user clock ins and outs
--

CREATE TABLE IF NOT EXISTS "TimeEntry" (
  "TimeEntryId" BIGSERIAL PRIMARY KEY,
  "UserId" INT NOT NULL,
  "AssignmentId" INT NULL,
  "StartTime" TIMESTAMPTZ NOT NULL DEFAULT now(),
  "EndTime" TIMESTAMPTZ NULL,

  CONSTRAINT fk_timeentry_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE,
  CONSTRAINT chk_timeentry_times CHECK ("EndTime" IS NULL OR "EndTime" >= "StartTime"),
  CONSTRAINT fk_timeentry_assignment FOREIGN KEY ("AssignmentId") REFERENCES "Assignment"("AssignmentId") ON DELETE SET NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS uq_timeentry_open_per_user ON "TimeEntry"("UserId") WHERE "EndTime" IS NULL;



DROP TABLE IF EXISTS "RequestOff";

CREATE TABLE IF NOT EXISTS "RequestOff" (
  "RequestOffId" BIGSERIAL PRIMARY KEY,
  "UserId"       INT NOT NULL,
  "StartDate"    DATE NOT NULL,
  "EndDate"      DATE NOT NULL,
  "Note"         VARCHAR,
  "Status"       VARCHAR,
  "CreatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now(),
  "UpdatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now(),

  CONSTRAINT fk_requestoff_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE,
  CONSTRAINT chk_requestoff_dates CHECK ("StartDate" <= "EndDate")
);

--
-- links which users are assigned to which jobsites
--

CREATE TABLE IF NOT EXISTS "UserAssignment" (
  "AssignmentId" INT NOT NULL, -- foreign key to Assignment
  "UserId" INT NOT NULL, -- foreign key to Users

  PRIMARY KEY ("AssignmentId", "UserId"), -- each pair is unique

  CONSTRAINT fk_userassignment_assignment FOREIGN KEY ("AssignmentId") REFERENCES "Assignment"("AssignmentId") ON DELETE CASCADE, -- deletes remove join rows if jobsite or user removed
  CONSTRAINT fk_userassignment_user FOREIGN KEY ("UserId") REFERENCES "Users"("UserId") ON DELETE CASCADE -- 
);

-- fast lookup indexes by user or assignment
CREATE INDEX IF NOT EXISTS idx_userassignment_user       ON "UserAssignment"("UserId");
CREATE INDEX IF NOT EXISTS idx_userassignment_assignment ON "UserAssignment"("AssignmentId");


-- storing comments made by user on an assignment

CREATE TABLE IF NOT EXISTS "AssignmentComment" (
  "CommentId"    SERIAL PRIMARY KEY,
  "AssignmentId" INT NOT NULL,
  "UserId"       INT NOT NULL,
  "Text"         TEXT NOT NULL,
  "CreatedAt"    TIMESTAMPTZ NOT NULL DEFAULT now(),

  CONSTRAINT fk_comment_assignment FOREIGN KEY ("AssignmentId")
    REFERENCES "Assignment"("AssignmentId") ON DELETE CASCADE,

  CONSTRAINT fk_comment_user FOREIGN KEY ("UserId")
    REFERENCES "Users"("UserId") ON DELETE CASCADE
);

-- indexes
CREATE INDEX IF NOT EXISTS idx_comment_assignment ON "AssignmentComment"("AssignmentId");
CREATE INDEX IF NOT EXISTS idx_comment_user       ON "AssignmentComment"("UserId");
