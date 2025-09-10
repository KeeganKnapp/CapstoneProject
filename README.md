

## Dashboard

```mermaid
flowchart TD
    A[Open App] -->|Log in| B(View Dashboard)
    B --> C(Pick Elements)
    C --> D[Progress]
    C --> E[Jobsites]
    C --> F[Logged Hours]
    C --> H[Requests]
    G(View Progress)
    D --> G
    E --> G
    F --> G
    H --> G
  ```
