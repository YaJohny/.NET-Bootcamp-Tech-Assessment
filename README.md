# Book Management API

## Overview
This is a **RESTful API** built using **ASP.NET Core Web API** that allows users to manage books. The API follows a **3-layered architecture** and supports **CRUD operations**, **JWT authentication**, and **pagination**.

## Features
- **Book Management** (Create, Read, Update, Delete)
- **Soft Delete Support**
- **JWT Authentication** for securing API endpoints
- **Pagination & Sorting** for retrieving books
- **Swagger API Documentation**
- **SQL Server with Entity Framework Core**

## Technologies Used
- **C#**
- **.NET 8 / .NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core (EF Core)**
- **SQL Server**
- **JWT Authentication**
- **Swagger for API Documentation**

## Installation Guide
### **Step 1: Clone the Repository**
```bash
git clone https://github.com/your-username/BookManagementAPI.git
cd BookManagementAPI
```

### **Step 2: Configure the Database**
Update the `appsettings.json` file with your **SQL Server** connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "BookAPI",
    "Audience": "BookAPIUsers"
  }
}
```

### **Step 3: Run Migrations & Update Database**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **Step 4: Run the Application**
```bash
dotnet run
```

The API will be accessible at: `https://localhost:5001/`

## API Endpoints
### **Authentication**
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive JWT token |

### **Books API**
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET | `/api/books` | Get all books (paginated & sorted by popularity) |
| GET | `/api/books/{id}` | Get book details (increments view count) |
| POST | `/api/books` | Add a new book |
| PUT | `/api/books/{id}` | Update book details |
| DELETE | `/api/books/{id}` | Soft delete a book |

## JWT Authentication Setup
The API requires authentication for all endpoints. After logging in, include the JWT token in your API requests:
```bash
Authorization: Bearer YOUR_ACCESS_TOKEN
```

## Swagger Documentation
The API is documented with **Swagger**. After running the project, access the Swagger UI at:
```
https://localhost:5001/swagger
```

## Contribution
Feel free to fork this repository, create a new branch, and submit a pull request.

## License
This project is licensed under the MIT License.

# Book Management API

## Overview
This is a **RESTful API** built using **ASP.NET Core Web API** that allows users to manage books. The API follows a **3-layered architecture** and supports **CRUD operations**, **JWT authentication**, and **pagination**.

## Features
- **Book Management** (Create, Read, Update, Delete)
- **Soft Delete Support**
- **JWT Authentication** for securing API endpoints
- **Pagination & Sorting** for retrieving books
- **Swagger API Documentation**
- **SQL Server with Entity Framework Core**

## Technologies Used
- **C#**
- **.NET 8 / .NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core (EF Core)**
- **SQL Server**
- **JWT Authentication**
- **Swagger for API Documentation**

## Installation Guide
### **Step 1: Clone the Repository**
```bash
git clone https://github.com/your-username/BookManagementAPI.git
cd BookManagementAPI
```

### **Step 2: Configure the Database**
Update the `appsettings.json` file with your **SQL Server** connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "BookAPI",
    "Audience": "BookAPIUsers"
  }
}
```

### **Step 3: Run Migrations & Update Database**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **Step 4: Run the Application**
```bash
dotnet run
```

The API will be accessible at: `https://localhost:5001/`

## API Endpoints
### **Authentication**
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive JWT token |

### **Books API**
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET | `/api/books` | Get all books (paginated & sorted by popularity) |
| GET | `/api/books/{id}` | Get book details (increments view count) |
| POST | `/api/books` | Add a new book |
| PUT | `/api/books/{id}` | Update book details |
| DELETE | `/api/books/{id}` | Soft delete a book |

## JWT Authentication Setup
The API requires authentication for all endpoints. After logging in, include the JWT token in your API requests:
```bash
Authorization: Bearer YOUR_ACCESS_TOKEN
```

## Swagger Documentation
The API is documented with **Swagger**. After running the project, access the Swagger UI at:
```
https://localhost:5001/swagger
```

## Contribution
Feel free to fork this repository, create a new branch, and submit a pull request.

## License
This project is licensed under the MIT License.

