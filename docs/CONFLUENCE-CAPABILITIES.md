# Atlassian Confluence — Knowledge Management Capabilities & Feature Reference

A comprehensive reference of all capabilities, features, and integrations available in Atlassian Confluence as a knowledge management platform.

---

## Table of Contents

1. [Platform Overview](#1-platform-overview)
2. [Content Types & Creation](#2-content-types--creation)
3. [Content Organization & Structure](#3-content-organization--structure)
4. [Real-Time Collaboration](#4-real-time-collaboration)
5. [AI & Atlassian Intelligence (Rovo)](#5-ai--atlassian-intelligence-rovo)
6. [Search & Discovery](#6-search--discovery)
7. [Templates & Blueprints](#7-templates--blueprints)
8. [Macros & Dynamic Content](#8-macros--dynamic-content)
9. [Commenting & Feedback](#9-commenting--feedback)
10. [Notifications & Watching](#10-notifications--watching)
11. [Automation](#11-automation)
12. [Permissions & Access Control](#12-permissions--access-control)
13. [Analytics & Reporting](#13-analytics--reporting)
14. [Audit Logging & Compliance](#14-audit-logging--compliance)
15. [Administration](#15-administration)
16. [Integrations](#16-integrations)
17. [Security & Enterprise Features](#17-security--enterprise-features)
18. [Mobile Access](#18-mobile-access)
19. [Storage & Scalability](#19-storage--scalability)
20. [Summary Metrics](#20-summary-metrics)

---

## 1. Platform Overview

Atlassian Confluence is a team workspace and knowledge management platform for creating, organizing, and sharing information. It is available as:

- **Confluence Cloud** — SaaS hosted by Atlassian (Free, Standard, Premium, Enterprise tiers)
- **Confluence Data Center** — Self-managed deployment for enterprises

### Technology & Deployment

| Aspect | Details |
|---|---|
| Deployment models | Cloud (SaaS), Data Center (self-hosted) |
| User capacity | Free: up to 10 users; Standard+: scales to 50,000+ |
| API | REST API for programmatic access |
| Marketplace | Thousands of third-party apps and extensions |
| Mobile | iOS and Android native apps |

---

## 2. Content Types & Creation

### Pages

| Capability | Description |
|---|---|
| Rich text editing | WYSIWYG editor with formatting, tables, images, code blocks |
| Real-time co-editing | Multiple contributors editing simultaneously with live sync |
| Page versioning | Every edit creates a new version; full change history with diff comparison |
| Version rollback | Revert to any previous version of a page |
| Version comments | Annotate each version with a summary of changes |
| Inline content | Embed images, GIFs, emojis, videos, code snippets, and dynamic macros |
| Draft support | Save drafts before publishing; unpublished drafts visible only to authors |
| Page status | Set and display content status (e.g., Draft, In Progress, Ready for Review) |
| Content owner | Assign a content owner displayed on the page |
| Read time | Automatically calculated estimated reading time |
| Classification | Classify content for governance purposes |
| Emoji reactions | Readers can react with emoji (beyond simple "like") |

### Live Docs

| Capability | Description |
|---|---|
| Frictionless editing | Always-editable documents — no publish step required |
| Automatic saving | All changes saved and synced in real time |
| Live collaboration | See co-editors' cursors and avatars in real time |
| Inline comments | Highlight text to leave targeted feedback |
| General comments | Page-level comments in a side panel |
| Emoji reactions | React to content with emoji |

### Blog Posts

| Capability | Description |
|---|---|
| Blog authoring | Time-stamped blog entries within any space |
| Blog listing | Chronological feed of blog posts per space |
| Comments & reactions | Same commenting and emoji reaction capabilities as pages |
| Notifications | Watchers notified of new blog posts |

### Whiteboards

| Capability | Description |
|---|---|
| Infinite canvas | Freeform visual brainstorming and ideation |
| Drawing tools | Shapes, connectors, freehand drawing |
| Sticky notes | Add, resize, and organize sticky notes |
| Smart sections | Group and organize content in sections |
| Jira integration | Convert whiteboard items to Jira issues; update Jira fields inline |
| External embeds | Embed Figma designs, Google Docs, YouTube videos |
| AI summarization | Generate summaries of whiteboard content |
| AI idea generation | Auto-generate sticky notes from existing content |
| AI grouping | Automatically organize stickies into thematic categories |
| Tiered access | Free: 3 active whiteboards; Standard: 10; Premium+: unlimited |

### Databases

| Capability | Description |
|---|---|
| Structured data | Create structured records with custom fields |
| Multiple views | Display data as tables, cards, or boards |
| Real-time sync | Live data synchronization across sources |
| Jira integration | Pull in and display Jira data |
| Third-party data | Aggregate from connected external tools |
| Filtering & sorting | Filter and sort by any field (status, owner, deadline, etc.) |

### Loom Video Integration

| Capability | Description |
|---|---|
| Screen recording | Record screen and camera directly within Confluence |
| Embedded playback | Videos embedded inline on pages, live docs, and whiteboards |
| Contextual placement | Place recordings exactly where context is needed |
| Async communication | Video messages for complex explanations across time zones |

---

## 3. Content Organization & Structure

### Spaces

| Capability | Description |
|---|---|
| Team spaces | Permanent hubs for departments (e.g., Engineering Wiki, HR Knowledge Hub) |
| Project spaces | Time-bound spaces for specific initiatives |
| Personal spaces | Private or public per-user workspaces |
| Space overview | Customizable landing page per space |
| Space sidebar | Configurable navigation sidebar with shortcuts |
| Space settings | Per-space color scheme, logo, and layout customization |
| Space permissions | Independent access control per space |
| Space archiving | Archive entire spaces to reduce clutter |

### Page Hierarchy

| Capability | Description |
|---|---|
| Page tree | Hierarchical parent-child page structure within each space |
| Nested pages | Unlimited nesting depth for sub-pages |
| Folders | Hierarchical folder structure for organizing content |
| Child pages macro | Display child pages as a navigable list |
| Content tree macro | Dynamic, hierarchical table of contents |
| Page reordering | Drag-and-drop page ordering within the tree |
| Page moving | Move pages between spaces or within the hierarchy |

### Labels & Categorization

| Capability | Description |
|---|---|
| Labels (tags) | Apply free-form labels to pages for cross-cutting categorization |
| Content by label | Macro to display all content matching specific labels |
| Label-based navigation | Browse content by label across spaces |
| Global labels | Labels visible and searchable site-wide |

### Homepage & Hub

| Capability | Description |
|---|---|
| Personalized homepage | Shows in-progress work, recent activity, drafts, and popular content |
| Organization hub | Central hub for news, announcements, and team resources |
| Popular content feed | Surface trending and frequently viewed content |
| Recent activity | Feed of recent edits, comments, and contributions |

---

## 4. Real-Time Collaboration

| Capability | Description |
|---|---|
| Simultaneous editing | Multiple users editing the same page at the same time |
| Live cursors | See other editors' cursor positions and selections |
| User presence | Avatars showing who is currently viewing/editing |
| Cross-timezone support | Asynchronous collaboration with edit history and notifications |
| @mentions | Tag team members inline to notify and assign |
| Task assignment | Create and assign tasks directly within page content |
| Page likes | Like pages to signal agreement or appreciation |
| Emoji reactions | React with emoji to express nuanced feedback |
| Share via link | Generate public or restricted links for external sharing |
| Anonymous access | Allow unauthenticated users to view specific content |

---

## 5. AI & Atlassian Intelligence (Rovo)

### Rovo Platform

| Capability | Description |
|---|---|
| Teamwork Graph | Data intelligence layer indexing all connected tools; understands relationships between people, projects, and goals |
| Cross-tool search | Search across Atlassian apps and connected third-party tools (Google Drive, Slack, etc.) simultaneously |
| Rovo Chat | Conversational AI for questions, brainstorming, and task execution using organizational knowledge |
| Rovo Agents | AI-powered virtual teammates with specialized knowledge and custom skills |
| Rovo Studio | Build custom AI agents with no-code/low-code |
| Actions in Chat | Create Jira issues, generate Confluence pages, visualize data, and more — directly from chat |

### Content AI Features

| Capability | Description |
|---|---|
| Content generation | Draft new pages, PRDs, strategy documents, and briefs from prompts |
| Content summarization | Summarize long pages, blog posts, and comment threads |
| Change summarization | Summarize what changed on a page since your last visit |
| Whiteboard summarization | Auto-generate summaries of whiteboard content |
| Comments recap | Summarize, analyze sentiment, and classify comments as informative or actionable |
| Writing enhancement | Improve grammar, clarity, and overall writing quality |
| Tone adjustment | Rewrite content for different audiences (professional, casual, empathetic, neutral) |
| Format transformation | Restructure content into different formats (notes → strategy docs, lists → tables) |
| Translation | Translate content into other languages |
| Idea generation | Generate new sticky notes and content ideas from existing material |
| Content grouping | Automatically organize content into thematic categories |

### Search & Knowledge AI

| Capability | Description |
|---|---|
| Q&A search (Beta) | Natural language question answering — ask questions like "What is our parental leave policy?" |
| Term definition (Beta) | Define organization-specific jargon, acronyms, and project names |
| Audio briefings | AI-generated audio summaries of content and updates |

### Task & Workflow AI

| Capability | Description |
|---|---|
| Jira task creation | Highlight text on a Confluence page and convert to a Jira issue |
| Automation rule generation | Describe an automation in natural language; AI creates the rule |

### Availability

Atlassian Intelligence / Rovo is available on **Premium and Enterprise** plans. Rovo is included in all paid Jira, Confluence, JSM, and Teamwork Collection subscriptions.

---

## 6. Search & Discovery

| Capability | Description |
|---|---|
| Global search | Search across all spaces, pages, blogs, attachments, and comments |
| AI-powered search | Rovo-enhanced search with natural language understanding |
| Cross-tool search | Search Confluence alongside Jira, Google Drive, Slack, and other connected tools |
| Advanced search | Filter by space, contributor, date range, content type, and labels |
| Quick search | Keyboard shortcut (`/`) for instant search overlay |
| Search suggestions | Autocomplete and typeahead as you type |
| Search highlighting | Matching terms highlighted in results |
| Attachment search | Search within uploaded file contents (PDF, Word, etc.) |
| Label-based browsing | Navigate content by label tags |
| Recently viewed | Quick access to your recently visited pages |
| Popular content | Surface frequently viewed and trending content |
| Smart Links | Hover-to-preview links to any Confluence or external content |

---

## 7. Templates & Blueprints

### Template System

| Capability | Description |
|---|---|
| Built-in templates | Pre-built templates across categories: Design, Finance/Ops, HR, Marketing, Product Management, Project Management, Software Development |
| Space templates | Templates available only within a specific space |
| Global templates | Templates available across all spaces site-wide |
| System templates | Default templates for site welcome messages and space content |
| Custom templates | Create your own templates with any content, macros, and layout |
| Template variables | Dynamic variables that prompt users to fill in values when creating from template |

### Blueprints

| Capability | Description |
|---|---|
| Meeting notes | Structured meeting notes with attendees, action items, and decisions |
| Shared file lists | Collaborative file listing and tracking |
| Requirements documentation | Product and project requirements capture |
| Retrospectives | Sprint/project retrospective templates |
| Decision documentation | DACI and decision-tracking templates |
| Project posters | Visual project summary templates |
| Health monitors | Team and project health assessment |
| Experience templates | Experience canvas and journey mapping |
| Auto-index pages | First use of a blueprint auto-creates an index page with sidebar shortcut |
| Marketplace blueprints | Download additional blueprints from the Atlassian Marketplace |
| Create from Template macro | Button on any page that opens the editor with a pre-loaded template |

---

## 8. Macros & Dynamic Content

Confluence includes 30+ built-in macros for embedding dynamic content and interactivity into pages.

### Content Display Macros

| Macro | Description |
|---|---|
| Children Display | Lists child pages hierarchically |
| Content by Label | Displays pages matching specific labels |
| Content by User | Shows content created by a specific user |
| Content Report Table | Generates tabular content reports |
| Page Tree | Interactive hierarchical page navigation |
| Include Page | Embeds the content of another page inline |
| Excerpt / Excerpt Include | Defines and reuses content excerpts across pages |
| Blog Posts | Displays a list of blog posts |
| Attachments | Lists file attachments on a page |
| Gallery | Displays attached images as a gallery |
| Favorite Pages | Shows user's bookmarked pages |

### Formatting & Layout Macros

| Macro | Description |
|---|---|
| Column | Creates multi-column page layouts |
| Panel | Highlighted text panels |
| Info / Tip / Note / Warning | Color-coded callout panels for different message types |
| Expand | Collapsible/expandable content sections |
| Code Block | Syntax-highlighted code display |
| Table of Contents | Auto-generated page TOC from headings |
| Anchor | Creates named anchor points for deep linking |

### Data & Reporting Macros

| Macro | Description |
|---|---|
| Chart | Embeds data visualizations (bar, line, pie, etc.) |
| Change History | Displays page modification history |
| Contributors / Contributors Summary | Shows page contributors and contribution stats |
| Content Report Table | Tabular reports of content metadata |
| Global Reports | Site-wide content and activity reports |

### Integration Macros

| Macro | Description |
|---|---|
| Jira Issues | Embeds live Jira issues with real-time status |
| Create from Template | Button to create a new page from a template |
| Create Space Button | Button to create a new space |
| HTML / HTML Include | Embed raw HTML or external HTML content |
| Widget Connector | Embed external content (YouTube, Vimeo, Google Maps, etc.) |

### Marketplace Macros

| Capability | Description |
|---|---|
| Third-party macros | Thousands of additional macros available via Atlassian Marketplace apps |
| User macros | System administrators can create custom macros |
| Connect macros | App developers can build custom macros via the Connect framework |

---

## 9. Commenting & Feedback

| Capability | Description |
|---|---|
| Page comments | General comments on any page, blog, or live doc |
| Inline comments | Highlight specific text to leave targeted comments |
| Threaded replies | Reply to existing comments for threaded discussions |
| Rich text comments | Bold, italic, underline, lists, links, and @mentions within comments |
| @mentions in comments | Tag users to notify them directly |
| Comment resolution | Mark inline comments as resolved |
| Comments panel | Unified panel to view, filter, and sort all comments (open, resolved, unread) |
| Emoji reactions | React to pages and comments with emoji |
| Likes | Like pages and blog posts |
| AI comment recap | Summarize all comments, analyze sentiment, classify as informative vs. actionable |

---

## 10. Notifications & Watching

| Capability | Description |
|---|---|
| Watch pages | Subscribe to email notifications for edits and comments on specific pages |
| Watch spaces | Subscribe to all activity within an entire space |
| Watch blog posts | Get notified of comments on blog posts |
| @mention notifications | Automatically notified when someone mentions you |
| Task assignment alerts | Notified when tasks are assigned to you |
| Email notifications | Configurable email digests and instant notifications |
| In-app notifications | Bell icon notification center within the Confluence UI |
| Notification preferences | Fine-grained control over which events trigger notifications |
| Version comment notifications | Watchers receive version comment descriptions via email |
| Quiet mode | Publish without notifying watchers |

---

## 11. Automation

| Capability | Description |
|---|---|
| Rule-based automation | "If This, Then That" automation rules with triggers, conditions, branches, and actions |
| Global rules | Site-wide automation rules managed by Confluence admins |
| Space rules | Per-space automation rules managed by space admins |
| Event triggers | Trigger on page publish, edit, comment, label change, etc. |
| Time-based triggers | Schedule actions at specific times or intervals |
| Slack/Teams actions | Send automated messages to Slack or Microsoft Teams channels |
| Task reminders | Auto-remind users about incomplete tasks |
| Approval workflows | Trigger follow-up tasks, review assignments, or approval requests after page publication |
| AI rule generation | Describe desired automation in natural language; AI creates the rule |
| Conditional logic | Branch rules based on page properties, labels, or other conditions |

---

## 12. Permissions & Access Control

### Permission Levels

| Level | Description |
|---|---|
| Site-level | Global permissions for all spaces and content |
| Space-level | Per-space permissions for viewing, editing, and administering |
| Page-level | Restrict individual pages to specific users or groups |
| Anonymous access | Allow unauthenticated users to view specific spaces or pages |

### Permission Capabilities

| Capability | Description |
|---|---|
| View restrictions | Restrict who can view a page or space |
| Edit restrictions | Restrict who can edit a page or space |
| Delete permissions | Control who can delete pages |
| Export permissions | Control who can export content |
| Admin permissions | Control who can administer space settings |
| Group-based permissions | Assign permissions to user groups |
| Individual permissions | Assign permissions to specific users |
| Granular split permissions | Four new fine-grained permission types for custom roles (2025+) |
| Inherited permissions | Child pages inherit parent page restrictions by default |
| Public links | Generate shareable links for external distribution |

---

## 13. Analytics & Reporting

| Capability | Plan | Description |
|---|---|---|
| Page views | Standard+ | View count per page |
| Page insights | Standard+ | Engagement metrics per page |
| Space analytics | Premium+ | Space-level activity and usage reports |
| Site analytics | Premium+ | Site-wide content and user engagement |
| User engagement filtering | Premium+ | Filter analytics by user, date range, and content type |
| Mission Control | Premium+ | Actionable insights for content improvement |
| Custom dashboards | Premium+ (via Jira/Atlassian Analytics) | Connect Confluence data with other tools for custom reporting using SQL queries |
| Content usage reports | All plans | Built-in reports on content freshness and activity |
| Analytics permissions | Admin | Control which groups can access analytics |

---

## 14. Audit Logging & Compliance

| Capability | Description |
|---|---|
| Audit log | Comprehensive history of key changes and events (permissions, configuration, user actions) |
| Space audit log | Per-space audit trail for permissions, configuration, and security events |
| Search & filter | Search audit logs by keyword, date, author, space, category, and summary |
| Export | Export audit log data for external analysis |
| Database storage | Audit logs stored in database for easy search and export |
| Log file output | Audit logs also written to file for third-party logging platform integration |
| Retention settings | Configurable retention period; maximum 10 million records in database |
| Role-based access | System admins, Confluence admins, and space admins can view relevant audit logs |
| Third-party integration | Log files designed for easy integration with SIEM and logging platforms (Splunk, Azure Sentinel, etc.) |

---

## 15. Administration

| Capability | Description |
|---|---|
| User management | Add, deactivate, and manage user accounts |
| Group management | Create and manage user groups |
| Space management | Create, archive, and delete spaces |
| Site configuration | Global look and feel, color schemes, and layouts |
| App management | Install, update, and manage Marketplace apps |
| Global templates | Manage site-wide page templates |
| Macro management | Enable, disable, and configure macros |
| Backup & restore | Site backup and restoration (Data Center) |
| Index management | Rebuild search indexes |
| Cache management | Clear and configure application caches |
| License management | Manage user licenses and subscription |
| Maintenance mode | Enable maintenance mode for upgrades |

---

## 16. Integrations

### Atlassian Suite

| Integration | Capabilities |
|---|---|
| **Jira Software** | Embed live Jira issues on pages; create Jira tickets from highlighted text; link pages to Jira tasks; display Jira roadmaps; real-time issue status sync |
| **Jira Service Management** | Power JSM knowledge base with Confluence spaces; self-service article surfacing; deflect up to 45% of support tickets |
| **Trello** | Link Trello boards and cards to Confluence pages |
| **Bitbucket** | Embed code snippets and repository information |
| **Loom** | Record and embed screen/video messages inline |
| **Atlassian Analytics** | Custom dashboards combining Confluence data with other Atlassian tools |
| **Atlassian Guard** | Centralized security, identity management, and policy enforcement |

### Third-Party Integrations

| Category | Examples |
|---|---|
| Communication | Slack, Microsoft Teams (notifications, sharing, automation actions) |
| Cloud storage | Google Drive, OneDrive, SharePoint, Dropbox, Box |
| Design | Figma (embedded live designs) |
| Video | YouTube, Vimeo (embedded via Widget Connector) |
| Diagrams | Draw.io, Lucidchart, Gliffy |
| Project management | Monday.com, Asana, Smartsheet |
| Developer tools | GitHub, GitLab |
| Identity | Okta, Azure AD, OneLogin (SSO/SCIM) |
| Logging/SIEM | Splunk, Azure Sentinel, ELK Stack |
| Marketplace | 3,000+ apps available on Atlassian Marketplace |

### API & Extensibility

| Capability | Description |
|---|---|
| REST API | Full programmatic access to spaces, pages, content, users, and permissions |
| Connect framework | Build custom apps and macros |
| Forge platform | Atlassian's cloud-native app development platform |
| Webhooks | Event-driven webhooks for external system notifications |
| OAuth 2.0 | Secure API authentication |

---

## 17. Security & Enterprise Features

### Authentication & Identity

| Capability | Plan |
|---|---|
| Username/password | All |
| SAML SSO | Enterprise (Cloud) / Data Center |
| SCIM provisioning & de-provisioning | Enterprise |
| Active Directory sync | Enterprise / Data Center |
| Multi-factor authentication (MFA) | All (enforceable at Enterprise) |
| Domain verification | Enterprise |
| 2-step verification enforcement | Enterprise |

### Data Protection

| Capability | Plan |
|---|---|
| Data encryption at rest and in transit | All Cloud |
| Data residency | Standard+ (choose data region) |
| IP allowlisting | Premium+ |
| Atlassian Guard (centralized security) | Enterprise |
| Email allowlisting | Enterprise |

### Compliance

| Capability | Description |
|---|---|
| SOC 2 Type II | Certified |
| ISO 27001 | Certified |
| HIPAA-ready | Configurable for healthcare compliance |
| GDPR compliance | Built-in data subject request handling |
| Audit logging | Full event logging for compliance investigations |
| Data retention policies | Configurable retention periods |

### Reliability

| Capability | Plan |
|---|---|
| 99.9% SLA | Premium |
| 99.95% SLA | Enterprise |
| Disaster recovery | Premium+ |
| Unlimited site instances | Enterprise (independent brands/departments) |

### Support

| Level | Plan |
|---|---|
| Community support | Free |
| Local business hours support | Standard |
| 24/7 support | Premium |
| 24/7 Enterprise support | Enterprise |

---

## 18. Mobile Access

| Capability | Description |
|---|---|
| iOS app | Native iPhone/iPad app via Apple App Store |
| Android app | Native app via Google Play Store |
| View & edit | Read and edit pages on mobile |
| Notifications | Push notifications for mentions, comments, and updates |
| Search | Full search capability on mobile |
| Offline access | View recently accessed pages offline |
| Cross-device sync | Changes sync seamlessly across desktop and mobile |

---

## 19. Storage & Scalability

| Plan | Storage | Users |
|---|---|---|
| Free | 2 GB | Up to 10 |
| Standard | 250 GB | Unlimited |
| Premium | Unlimited | Unlimited |
| Enterprise | Unlimited | Unlimited |

### Content Limits

| Capability | Description |
|---|---|
| Unlimited spaces | All plans (Free: up to 10 active whiteboards) |
| Unlimited pages | No page count limit on any plan |
| File attachments | Upload and attach files to any page |
| Attachment size limits | Configurable per site (default varies by plan) |

---

## 20. Summary Metrics

| Metric | Value |
|---|---|
| Content types | 5 (Pages, Live Docs, Blog Posts, Whiteboards, Databases) |
| Built-in macros | 30+ |
| Marketplace apps | 3,000+ |
| Template categories | 7+ (Design, Finance, HR, Marketing, Product, Project, Software Dev) |
| Permission levels | 4 (Site, Space, Page, Anonymous) |
| AI capabilities | 14+ (via Rovo/Atlassian Intelligence) |
| Deployment models | 2 (Cloud, Data Center) |
| Cloud plan tiers | 4 (Free, Standard, Premium, Enterprise) |
| Mobile platforms | 2 (iOS, Android) |
| Atlassian integrations | 7+ (Jira, JSM, Trello, Bitbucket, Loom, Analytics, Guard) |
| Third-party integrations | 3,000+ via Marketplace |
| SSO support | SAML 2.0, Azure AD, Okta, OneLogin |
| Compliance certifications | SOC 2 Type II, ISO 27001, HIPAA-ready, GDPR |

---

## Sources

- [Confluence Features — What's New](https://www.atlassian.com/software/confluence/features)
- [Knowledge Management with Confluence](https://www.atlassian.com/software/confluence/use-cases/knowledge-management-software)
- [AI Features in Confluence](https://support.atlassian.com/organization-administration/docs/atlassian-intelligence-features-in-confluence/)
- [Atlassian AI in Confluence](https://www.atlassian.com/software/confluence/resources/guides/best-practices/atlassian-ai)
- [Rovo Features](https://www.atlassian.com/software/rovo/features)
- [Confluence Automation](https://support.atlassian.com/confluence-cloud/docs/what-is-confluence-automation/)
- [Confluence Macros](https://confluence.atlassian.com/doc/macros-139387.html)
- [Confluence Spaces](https://confluence.atlassian.com/doc/spaces-139459.html)
- [Auditing in Confluence](https://confluence.atlassian.com/doc/auditing-in-confluence-829076528.html)
- [Confluence Blueprints](https://confluence.atlassian.com/doc/blueprints-323982376.html)
- [Live Docs in Confluence](https://support.atlassian.com/confluence-cloud/docs/create-and-collaborate-in-real-time-with-live-docs/)
- [Confluence Review 2026](https://www.eesel.ai/blog/confluence-review)
- [What Is Confluence — Complete Guide 2026](https://ikuteam.com/blog/what-is-confluence)
- [Confluence for Document Management 2026](https://ikuteam.com/blog/confluence-for-document-management)
- [Confluence and JSM Integration](https://www.atlassian.com/software/confluence/resources/guides/extend-functionality/confluence-jsm)
