# 📚 Book Management API

## 🚀 Overview
This is a **RESTful API** built with **ASP.NET Core Web API** for managing books. It follows a **3-layered architecture** and supports **CRUD operations**, **JWT authentication**, and **pagination**.

## 🔥 Features
- 📖 **Book Management** (Create, Read, Update, Delete)  
- 🗑️ **Soft Delete Support**  
- 🔐 **JWT Authentication** for security  
- 📊 **Pagination & Sorting** for efficient book retrieval  
- 📜 **Swagger API Documentation**  
- 🗄️ **SQL Server with Entity Framework Core (EF Core)**  

## 🛠️ Technologies Used
- 💻 **C#**
- 🌐 **.NET 9**
- ⚙️ **ASP.NET Core Web API**
- 🗃️ **Entity Framework Core (EF Core)**
- 🛢️ **SQL Server**
- 🔑 **JWT Authentication**
- 📑 **Swagger for API Documentation**

---

## 👥 User Roles
- 👤 **User**: Can view and access public book information.
- 🔧 **Admin**: Can manage books (add, update, delete), promote users(to Admin or Librarian), and perform administrative tasks.
- 📚 **Librarian** (Upcoming Role): Will have all current admin rights(except make-admin).

---

## ⚙️ Installation Guide

### 📂 Step 1: Clone the Repository
```bash
git clone https://github.com/your-username/BookManagementAPI.git
cd BookManagementAPI
```

### 🔑 Step 2: Configure User Secrets
Run the following command to initialize user secrets:
```bash
dotnet user-secrets init
```

Then, add the required secrets:
```bash
dotnet user-secrets set "DefaultAdminUser:Password" "<value>"
dotnet user-secrets set "DefaultAdminUser:Email" "<value>"
dotnet user-secrets set "Database:ConnectionString" "<value>"
dotnet user-secrets set "Kestrel:Certificates:Development:Password" "<value>"
dotnet user-secrets set "Jwt:Secret" "<value>"
dotnet user-secrets set "Jwt:Issuer" "<value>"
dotnet user-secrets set "Jwt:Audience" "<value>"
```

### 🛠️ Step 3: Apply Migrations & Update Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### ▶️ Step 4: Run the Application
```bash
dotnet run --urls=http://localhost:5153
```

API will be accessible at:  
**`http://localhost:5153/`**

---

## 🔗 API Endpoints

### 🔑 Authentication
| Method | Endpoint | Description |
|--------|---------|-------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Authenticate and receive JWT token |
| `POST` | `/api/auth/make-admin` | Promote a user to admin role (requires admin privileges) |

### 📚 Books API
| Method | Endpoint | Description | Accessible By |
|--------|---------|-------------|---------------|
| `GET` | `/api/books` | Get all books (paginated & sorted by popularity) | All users |
| `GET` | `/api/books/{id}` | Get book details (increments view count) | All users |
| `POST` | `/api/books` | Add a new book | Admin only |
| `PUT` | `/api/books/{id}` | Update book details | Admin only |
| `DELETE` | `/api/books/{id}` | Soft delete a book | Admin only |
| `GET` | `/api/books/ranking` | Get book rankings sorted by popularity | All users |

---

## 🔐 JWT Authentication Setup
All endpoints require authentication. After logging in, include the JWT token in your requests:
```http
Authorization: Bearer <value>
```

---

## 📑 Swagger Documentation
The API is fully documented with **Swagger**.  
After running the project, access the **Swagger UI** at:
```
http://localhost:5153/
```

Swagger includes an **Authorize** button where you can provide your Bearer token to access protected endpoints.

---

## 🚀 Further Development
- 📚 **Librarian Role**: A new role with all admin privileges.
- 🔧 **New Admin Privileges**: Admin users will gain additional rights to hard delete books.
- ⚙️ **Advanced Permissions**: Fine-grained control over who can manage different book-related actions.

---

### 🎉 Happy Coding!

