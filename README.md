# EF Core Best Practices

This repository demonstrates Entity Framework Core best practices through practical API endpoints. Each endpoint (except the first two basic CRUD operations) showcases specific EF Core optimization techniques and patterns.

## Overview

The project contains a BookStore API that demonstrates various EF Core concepts and best practices for performance, tracking, querying, and data management.

## Endpoints Documentation

### 📚 Basic CRUD Operations

#### `GET /api/Books`
**Purpose**: Basic read operation with relationship loading  
**Description**: Retrieves all books with authors and genre information using projection to anonymous types for better performance.

**Key Features**:
- Uses `AsNoTracking()` for read-only operations
- Projection to anonymous types for reduced data transfer
- Includes related data (Genre, Authors) efficiently

#### `GET /api/Books/{id}`
**Purpose**: Basic single entity retrieval  
**Description**: Gets a specific book by ID with included Genre information.

**Key Features**:
- Uses `AsNoTracking()` for read-only operations
- Proper null handling with typed results
- Eager loading with `Include()`

---

### 🎯 EF Core Best Practices Endpoints

#### `GET /api/Books/untracked`
**Best Practice**: Untracked Queries for Read-Only Operations  
**Description**: Demonstrates different ways to perform untracked queries that don't load entities into the change tracker.

**Key Concepts**:
- `AsNoTracking()` - Explicitly untracked queries
- Anonymous type projection - Automatically untracked
- DTO projection - Untracked and type-safe
- **Performance Benefit**: Reduced memory usage and faster queries for read-only scenarios

#### `GET /api/Books/tracked`
**Best Practice**: Understanding Change Tracking  
**Description**: Shows how EF Core tracks entities and when tracking is necessary vs. unnecessary.

**Key Concepts**:
- Change tracker monitoring with `db.ChangeTracker.Entries()`
- Different query types and their tracking behavior
- When to use tracking (for updates) vs. when to avoid it (read-only)
- **Memory Management**: Understanding tracked entity lifecycle

#### `GET /api/Books/update`
**Best Practice**: Efficient Update Strategies  
**Description**: Compares different approaches to updating data in the database.

**Key Concepts**:
- Traditional tracked entity updates
- `ExecuteUpdateAsync()` for bulk updates without loading data into memory
- Performance comparison between approaches
- **Performance Benefit**: Bulk updates without entity materialization

#### `GET /api/Books/bulkupdate`
**Best Practice**: Bulk Operations  
**Description**: Demonstrates challenges with bulk updates in EF Core and mentions third-party solutions.

**Key Concepts**:
- EF Core's limitations with bulk operations
- `UpdateRange()` usage and performance implications
- Third-party bulk operation libraries
- **Scale Consideration**: When to use external tools for large datasets

#### `GET /api/Books/firstordefault`
**Best Practice**: Server vs Client-Side Evaluation  
**Description**: Shows the difference between server-side and client-side query evaluation.

**Key Concepts**:
- Server-side evaluation (translated to SQL)
- Client-side evaluation (executed in memory)
- Performance implications of each approach
- **Performance Impact**: Understanding where query logic executes

#### `GET /api/Books/clientvsserverside`
**Best Practice**: Query Translation Boundaries  
**Description**: Demonstrates the transition point between server and client evaluation using `AsAsyncEnumerable()`.

**Key Concepts**:
- `AsAsyncEnumerable()` as transition point
- Mixed server/client evaluation in single query
- Local function execution on client-side
- **Architecture Understanding**: Query pipeline execution flow

#### `GET /api/Books/streaming`
**Best Practice**: Streaming vs Buffering  
**Description**: Shows different approaches to processing large datasets without loading everything into memory.

**Key Concepts**:
- Synchronous streaming with `foreach`
- Asynchronous streaming with `await foreach`
- `AsEnumerable()` vs `ToList()` for memory management
- **Memory Efficiency**: Processing large datasets incrementally

#### `GET /api/Books/withidentity`
**Best Practice**: Identity Resolution for Untracked Queries  
**Description**: Demonstrates `AsNoTrackingWithIdentityResolution()` for maintaining object references in untracked scenarios.

**Key Concepts**:
- Object identity in untracked queries
- Shared references for related entities
- When to use identity resolution
- **Memory Optimization**: Reducing object duplication in complex graphs

#### `GET /api/Books/cartesianexploasion`
**Best Practice**: Avoiding Cartesian Explosion  
**Description**: Shows how to use `AsSplitQuery()` to prevent cartesian explosion when including multiple related collections.

**Key Concepts**:
- Cartesian explosion problem with multiple `Include()`
- `AsSplitQuery()` to split into multiple queries
- Data transfer optimization
- **Network Efficiency**: Reducing redundant data transfer

#### `GET /api/Books/dbfunctions`
**Best Practice**: Database Function Usage  
**Description**: Demonstrates using database-specific functions through `EF.Functions`.

**Key Concepts**:
- `EF.Functions` for database-specific operations
- Server-side function execution
- Cross-database compatibility considerations
- **Performance**: Leveraging database capabilities

#### `GET /api/Books/cancellationtoken`
**Best Practice**: Cancellation Support  
**Description**: Shows proper use of cancellation tokens for long-running queries.

**Key Concepts**:
- `CancellationToken` parameter injection
- Async operation cancellation
- Resource cleanup and timeout handling
- **Reliability**: Graceful operation cancellation

#### `GET /api/Books/globalqueryfilters`
**Best Practice**: Global Query Filters  
**Description**: Demonstrates global query filters for cross-cutting concerns like soft deletes or tenant isolation.

**Key Concepts**:
- Automatic filtering with global query filters
- `IgnoreQueryFilters()` to bypass filters
- Configuration in entity type builder
- **Architecture**: Implementing cross-cutting concerns

#### `GET /api/Books/enumproblem`
**Best Practice**: Enum Default Value Handling  
**Description**: Explains the enum default value problem and solution using sentinel values.

**Key Concepts**:
- Enum default value (0) vs database default
- `HasSentinel()` configuration
- Distinguishing between app defaults and DB defaults
- **Data Integrity**: Proper enum value handling

#### `GET /api/Books/rowversions`
**Best Practice**: Optimistic Concurrency Control  
**Description**: Explains row version implementation for optimistic concurrency.

**Key Concepts**:
- Row version columns for concurrency control
- Automatic conflict detection
- 8-byte opaque version values
- **Data Consistency**: Preventing lost updates

## Key Takeaways

### Performance Optimization
- Use `AsNoTracking()` for read-only queries
- Prefer projection over full entity loading
- Use `ExecuteUpdateAsync()` for bulk operations
- Consider `AsSplitQuery()` for complex includes

### Memory Management
- Understand tracking vs non-tracking implications
- Use streaming for large datasets
- Clear change tracker when appropriate
- Use identity resolution judiciously

### Query Efficiency
- Keep computation on the server side when possible
- Use appropriate database functions
- Implement proper cancellation support
- Leverage global query filters for cross-cutting concerns

### Data Integrity
- Implement optimistic concurrency with row versions
- Handle enum defaults properly with sentinels
- Use proper update strategies for different scenarios

## Running the Project

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations
4. Start the API and explore the endpoints

Each endpoint is documented with OpenAPI/Swagger for interactive testing.

## Technologies Used

- .NET 8
- Entity Framework Core
- ASP.NET Core Minimal APIs
- SQL Server
- OpenAPI/Swagger

## Contributing

Feel free to contribute additional EF Core best practices or improvements to existing examples.
