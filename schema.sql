

DROP TABLE IF EXISTS employee;
CREATE TABLE employee (
  employeeId int NOT NULL,
  username varchar NOT NULL UNIQUE,
  password varchar NOT NULL,
  firstName varchar NOT NULL,
  management bool NOT NULL,
  PRIMARY KEY(employeeId)
);

DROP TABLE IF EXISTS assignment;
CREATE TABLE assignment (
  assignmentId int NOT NULL,
  -- I've chosen point to store the location of the job site as it should suit our needs
  -- If we need something more advanced the extention PostGIS specializes in storing and working with geospatial data
  site point NOT NULL,
  totalHours int NOT NULL,
  archived bool NOT NULL,
  PRIMARY KEY(assignmentId)
);

DROP TABLE IF EXISTS userAssignment;
CREATE TABLE userAssignment (
  assignmentId int NOT NULL,
  employeeId int NOT NULL,
  PRIMARY KEY(assignmentId, employeeId),
  FOREIGN KEY (assignmentId) REFERENCES assignment(assignmentId) ON DELETE CASCADE,
  FOREIGN KEY (employeeId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

DROP TABLE IF EXISTS request;
CREATE TABLE request (
	requestId int NOT NULL,
	requestType varchar NOT NULL,
	requestTimeStart date,
	requestTimeEnd date,
	requestNote varchar,
  PRIMARY KEY(requestId)
);

DROP TABLE IF EXISTS userRequest;
CREATE TABLE userRequest (
  requestId int NOT NULL,
  employeeId int NOT NULL,
  PRIMARY KEY(requestId, employeeId),
  FOREIGN KEY (requestId) REFERENCES request(requestId) ON DELETE CASCADE,
  FOREIGN KEY (employeeId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

DROP TABLE IF EXISTS notification;
CREATE TABLE notification (
  notificationId int NOT NULL,
  senderId int NOT NULL,
  notifType varchar NOT NULL,
  notifContents varchar NOT NULL,
  PRIMARY KEY(notificationId),
  FOREIGN KEY (senderId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

DROP TABLE IF EXISTS userRecipient;
CREATE TABLE userRecipient (
  notificationId int NOT NULL,
  recipientId int NOT NULL,
  PRIMARY KEY(notificationId,recipientId),
  FOREIGN KEY (notificationId) REFERENCES notification(notificationId) ON DELETE CASCADE,
  FOREIGN KEY (recipientId) REFERENCES employee(employeeId) ON DELETE CASCADE
);

