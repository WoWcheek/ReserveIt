
# Meeting Rooms Reservation API

A reservation system built on ASP.NET Core, providing a RESTful API for managing meeting rooms and their bookings.

## Contents

- [Overview](#overview)
- [Technologies](#technologies)
- [Features](#features)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Usage Examples](#usage-examples)
- [Business Rules and Constraints](#business-rules-and-constraints)
- [Error Handling](#error-handling)

## Overview

Room Booking API is a service that allows users to create meeting rooms and book them for specific time slots. The system provides basic authentication, data validation, and prevents booking conflicts.

## Technologies

- .NET 8
- ASP.NET Core
- Entity Framework Core
- Microsoft SQL Server
- xUnit, FluentAssertions for testing
- Swagger for API documentation

## Features

- **Meeting Room Management**: Create and retrieve meeting rooms information
- **Booking System**: Create, retrieve, and delete bookings
- **Conflict Prevention**: Overlapping bookings for the same room are denied
- **Authentication**: JWT token-based security
- **Validation**: Input data validation according to business rule
- **Error Handling**: Custom exceptions with appropriate HTTP status codes

## Getting Started

### Prerequisites

- .NET 8 SDK  
- SQL Server
- Git (for cloning the repository)  

### Setup Steps

1. **Clone the repository**

```bash
git clone https://github.com/WoWcheek/ReserveIt.git
cd ReserveIt
```

2. **Configure the database**

Edit the connection string in the `appsettings.json` file in `ReserveIt.Presentation` folder:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<put-your-server-name-here>;Database=<put-your-database-name-here>;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

3. **Apply migrations**

```bash
cd ReserveIt.Presentation
dotnet ef database update
```

4. **Run the project**

```bash
dotnet run
```

The application will be available at:  
https://localhost:5245  

Swagger UI:  
https://localhost:5245/swagger

## API Endpoints

### Authentication

- **POST /api/auth/register** - Register a new user  
- **POST /api/auth/login** - Log in and obtain a JWT token  

### Rooms

- **GET /api/rooms** - Get a list of all rooms  
- **GET /api/rooms/{id}** - Get information about a specific room  
- **POST /api/rooms** - Create a new room  

### Bookings

- **GET /api/bookings** - Get a list of all bookings  
- **GET /api/bookings/{id}** - Get information about a specific booking  
- **POST /api/bookings** - Create a new booking  
- **DELETE /api/bookings/{id}** - Delete an existing booking  

## Usage Examples

### Register a User

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "password": "Password123!"
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Password123!"
}
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Create a Room

```http
POST /api/rooms
Content-Type: application/json
Authorization: Bearer {your_token}

{
  "name": "Big Room",
  "capacity": 25
}
```

Response:

```json
{
  "id": 1,
  "name": "Big Room",
  "capacity": 25
}
```

### Get All Rooms

```http
GET /api/rooms
Authorization: Bearer {your_token}
```

Response:

```json
[
  {
    "id": 1,
    "name": "Big Room",
    "capacity": 25
  },
  {
    "id": 2,
    "name": "Medium Room",
    "capacity": 10
  }
]
```

### Create a Booking

```http
POST /api/bookings
Content-Type: application/json
Authorization: Bearer {your_token}

{
  "roomId": 1,
  "startTime": "2023-12-10T10:00:00",
  "endTime": "2023-12-10T12:00:00",
  "bookedBy": "Ivan Franko"
}
```

Response:

```json
{
  "id": 1,
  "roomId": 1,
  "roomName": "Big Room",
  "startTime": "2023-12-10T10:00:00",
  "endTime": "2023-12-10T12:00:00",
  "bookedBy": "Ivan Franko"
}
```

### Get All Bookings

```http
GET /api/bookings
Authorization: Bearer {your_token}
```

Response:

```json
[
  {
    "id": 1,
    "roomId": 1,
    "roomName": "Big Room",
    "startTime": "2023-12-10T10:00:00",
    "endTime": "2023-12-10T12:00:00",
    "bookedBy": "Ivan Franko"
  },
  {
    "id": 2,
    "roomId": 2,
    "roomName": "Medium Room",
    "startTime": "2023-12-11T14:00:00",
    "endTime": "2023-12-11T15:30:00",
    "bookedBy": "Taras Shevchenko"
  }
]
```

### Delete a Booking

```http
DELETE /api/bookings/1
Authorization: Bearer {your_token}
```

## Business Rules and Constraints

- Rooms cannot be booked in the past
- The end time must be later than the start time
- A room cannot be double-booked for overlapping time periods
- All API requests (except authentication) require a valid JWT token

## Error Handling

The API returns appropriate HTTP status codes and error messages:

- `400 Bad Request` - Invalid input data or business rule violation  
- `401 Unauthorized` - Missing or invalid authentication token  
- `404 Not Found` - Requested resource doesn't exist  
- `409 Conflict` - Resource conflict (e.g., overlapping bookings)  
- `500 Internal Server Error` - Unexpected server error  

Example error response:

```json
{
  "error": {
    "message": "Room is already booked for this time",
    "detail": null
  }
}
```
