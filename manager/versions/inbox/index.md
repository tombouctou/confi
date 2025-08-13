# Versions Inbox

This is an unstructured box of untriaged features i.e. features that are neither assigned to a specific version and may not be investigated enough.

> 💡 A feature here **can** be even partially implemented, since sometimes features intertwine.

## The Checklist

- [ ] Configuration Value
    - [x] Manager Accepts Configuration Values (by App Id)
    - [ ] Consumer Polls Current Configuration from the Manager's API
    - [ ] UI Configuration Value
        - [ ] See app configuration value (by id)
        - [ ] Edit an app configuration value
- [ ] Event-Based Configuration Value Refreshing
- [ ] Consumer Self-Declaration
    - [ ] Declares App (AppId)
    - [ ] Declares Schema & Initial Value
        - [ ] Declares Schema
        - [ ] Extracts Value Based on the Schema
- [ ] Admin App Declaration & Enrichment
- [ ] Schema Support
    - [ ] Manager Schema Support
        - [x] App Schema Uploading
        - [ ] Configuration Schema-Based Validation. (not allows extras)
    - [ ] Consumer Schema Support
        - [ ] Self-Declares Schema
    - [ ] UI Schema Support
        - [ ] Validate Configuration
- [ ] Form-Based UI Editing
    - [ ] Base Fields Support
    - [ ] Support for `additionalProperties`
- [ ] Schema Versioning Support
    - [ ? ] Manager Only Schema Updates with Newer Version
- [ ] Nodes Support
    - [x] Manager Accepts Current Node Configuration
    - [ ] Consumer Notifies About Current Node Configuration
    - [ ] UI Displays Node Status
- [ ] Authorization
    - [ ] Server-2-Server
    - [ ] User-2-UI
- [ ] Configuration Value Versioning (rollbacks, who changed, etc).