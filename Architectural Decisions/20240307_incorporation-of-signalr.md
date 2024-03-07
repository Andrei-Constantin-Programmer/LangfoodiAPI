# Incorporation of SignalR for real-time communication

## Status
Accepted

## Context
Prior to integrating SignalR, our API solely offered static endpoints, lacking dynamic interaction between the API and clients. SignalR presents an opportunity to implement real-time updates from the backend to the frontend, particularly enhancing live messaging functionalities.

## Decision
It has been decided to integrate SignalR into our infrastructure to facilitate live communication between users and the API.

## Consequences

### Positive
- **Real-time updates** - Highly enhanced user experience and improved system visibility.
- **Improved responsiveness** - The users will get real-time feedback to their (and other users') actions.

### Negative
- **Increased complexity** - There will be some overhead in introducing this change and making sure existing functionality is integrated with SignalR.
