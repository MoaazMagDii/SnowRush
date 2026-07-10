## SnowRush

Many small and medium-sized businesses struggle to build a scalable and secure e-commerce backend capable of handling product management, user authentication, shopping carts, online payments, and order processing.

Traditional implementations often suffer from tightly coupled architecture, insecure authentication mechanisms, poor scalability, and difficult maintenance.

SnowRush addresses these challenges by providing a modern RESTful backend built with ASP.NET Core that follows industry best practices. The system offers role-based authorization, Stripe payment integration, Cloudinary image management, efficient database access using Entity Framework Core, and a modular architecture that is easy to extend and maintain.

---

## Features

- User Registration & Login
- Role-Based Authorization
- Product Management
- Shopping Cart
- Order Management
- Stripe Payment Integration
- Cloudinary Image Upload
- Pagination
- Filtering, Sorting, and Searching
- AutoMapper
- Global Exception Handling
- Postman Documentation

---

## Tech Stack

### Backend

- ASP.NET Core Web API
- C#
- Entity Framework Core

### Database

- Entity Framework Core is used with SQLite.
  
### Authentication

- ASP.NET Identity
  
### Payments

Integrated with **Stripe** to support secure online payments.

### Image Upload

Product images are collected and stored securely using **Cloudinary**.

- https://res.cloudinary.com/dywlgfpbc/image/upload/v1782766814/glove-code1_jjctpf.webp
- https://res.cloudinary.com/dywlgfpbc/image/upload/v1782766813/boot-ang1_ft57yo.webp
- https://res.cloudinary.com/dywlgfpbc/image/upload/v1782766813/hat-core1_cz3lad.webp
- https://res.cloudinary.com/dywlgfpbc/image/upload/v1782766812/sb-ang1_kzg0dp.webp

---

## API Documentation

The API endpoints are documented using a Postman collection.

- Import `Postman/SnowRush.postman_collection.json` into Postman to test the API.
- The collection includes authentication, products, basket, orders, and payment endpoints with example requests.
- https://documenter.getpostman.com/view/29665995/2sBY4JxNga

---

## 📄 License

This project is created for educational and portfolio purposes.
