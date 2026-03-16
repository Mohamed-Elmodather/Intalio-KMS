# Guru — Knowledge Management Capabilities & Feature Reference

A comprehensive reference of all capabilities, features, and integrations available in Guru as an AI-powered knowledge management platform.

---

## Table of Contents

1. [Platform Overview](#1-platform-overview)
2. [Content Creation & Editing](#2-content-creation--editing)
3. [Content Organization & Structure](#3-content-organization--structure)
4. [Knowledge Verification & Quality](#4-knowledge-verification--quality)
5. [AI Knowledge Agents](#5-ai-knowledge-agents)
6. [AI Search & Discovery](#6-ai-search--discovery)
7. [AI Content Assist](#7-ai-content-assist)
8. [Intranet & Company Hub](#8-intranet--company-hub)
9. [Contextual Knowledge Delivery](#9-contextual-knowledge-delivery)
10. [Announcements & Communication](#10-announcements--communication)
11. [Collaboration](#11-collaboration)
12. [Publishing Workflows](#12-publishing-workflows)
13. [Analytics & Insights](#13-analytics--insights)
14. [Permissions & Access Control](#14-permissions--access-control)
15. [Integrations](#15-integrations)
16. [Security & Compliance](#16-security--compliance)
17. [Administration](#17-administration)
18. [API & Extensibility](#18-api--extensibility)
19. [Pricing & Plans](#19-pricing--plans)
20. [Summary Metrics](#20-summary-metrics)

---

## 1. Platform Overview

Guru is an AI-powered knowledge management platform that serves as an organization's single source of truth. It centralizes company information into searchable, verifiable "knowledge cards" and delivers trusted answers directly within workplace tools.

### Core Pillars

| Pillar | Description |
|---|---|
| Enterprise AI Search | Semantic, federated search across Guru and connected apps |
| Knowledge Agents | AI-powered virtual assistants that answer, chat, and research using governed company knowledge |
| Verified Knowledge Base | Wiki-style knowledge cards with automated verification workflows |
| Intranet | Custom pages, announcements, org chart, and employee profiles |
| Contextual Delivery | Browser extension, Slack/Teams integration, and knowledge triggers that surface information in the flow of work |

### Deployment

| Aspect | Details |
|---|---|
| Deployment model | Cloud (SaaS) only |
| Access methods | Web app, browser extension (Chrome/Edge/Opera), Slack, Microsoft Teams, API |
| Content unit | Knowledge Cards (bite-sized, focused content items) |
| AI model | Private AI model; zero data retention by third-party LLMs |

---

## 2. Content Creation & Editing

### Collaborative Editor

| Capability | Description |
|---|---|
| Rich text editing | Full formatting: headings, bold, italic, underline, strikethrough, text color, font size |
| Tables | Build and format tables with styled text, images, and links inside cells; full-width expansion |
| Media embedding | Embed images, videos, GIFs, and external content inline |
| Code blocks | Syntax-highlighted code snippets |
| Callouts | Info, tip, warning, and note callout panels |
| Lists | Bulleted, numbered, and checklist (task) lists |
| Links & anchors | Internal card links, external URLs, and anchor points |
| Dividers | Horizontal dividers for section separation |
| Layouts | Multi-column layouts for structured content |
| Real-time co-editing | Multiple authors writing, revising, and commenting simultaneously |
| Inline comments | Leave targeted comments on specific text selections |
| Change tracking | View edit history and track changes without version control headaches |
| Auto-save | Automatic saving of all edits |

### Content Templates

| Capability | Description |
|---|---|
| Pre-built templates | Ready-made templates with sample content, structure, and formatting |
| Custom templates | Create organization-specific templates with custom layouts |
| Default verifiers | Templates can auto-assign a default verifier and verification interval |
| Default placement | Templates can suggest the right Collection and folder placement |
| Recommended titles | Templates can set recommended title patterns |
| Template library | Browse and select from available templates when creating new cards |

### Import & Sync

| Capability | Description |
|---|---|
| Content sync | Sync content from external sources (Google Drive, Confluence, etc.) — stays auto-updated |
| Bulk import | Import existing documentation in bulk |
| HRIS sync | Import employee data from BambooHR, Workday, Gusto, etc. |
| Migration support | Tools for migrating from other knowledge platforms |

---

## 3. Content Organization & Structure

### Hierarchy

| Level | Description |
|---|---|
| Collections | Top-level containers for a team's knowledge; group content by department, topic, or function |
| Folders | Sub-divisions within Collections; nestable up to 3 levels deep |
| Cards | Individual knowledge items — focused, digestible content pieces |
| Tags/Labels | Cross-cutting categorization applied to cards |

### Organization Features

| Capability | Description |
|---|---|
| Collections | Primary organizational units with independent access controls |
| Nested folders | Up to 3 levels of folder nesting within Collections |
| Card linking | Cross-reference cards with internal links |
| Favorites | Bookmark frequently accessed cards |
| Recently viewed | Quick access to recently opened content |
| Card metadata | Owner, verifier, verification status, created/modified dates, tags |
| Bulk operations | Move, tag, and manage multiple cards at once |

---

## 4. Knowledge Verification & Quality

### Verification Workflow

| Capability | Description |
|---|---|
| Verifier assignment | Assign a subject matter expert (SME) as the verifier for each card |
| Verification interval | Set custom review cycles per card (e.g., every 30, 60, 90 days) |
| Automated reminders | System sends automated reminders when verification is due (via Slack, email) |
| One-click re-verification | SME can confirm accuracy with a single click |
| Unverified flagging | Cards past their verification date are visually flagged as unverified |
| Knowledge decay indicator | Cards turn yellow after the verification period expires |
| SME re-routing | If the verifier doesn't respond, notifications are escalated |
| Verification history | Full audit trail of verification actions |
| Trust score | Analytics dashboard tracks verification health across the knowledge base |

### Knowledge Quality Automation (AI-Powered)

| Capability | Description |
|---|---|
| Continuous evaluation | AI continuously evaluates content using usage signals, content context, and policy-driven rules |
| Auto-verify | Automatically verifies content that meets quality criteria |
| Auto-unverify | Automatically flags outdated or questionable information |
| Decision logging | Every automated decision is logged with full transparency |
| Human override | Admins retain full visibility and can override any automated decision |
| Stale content prevention | Prevents outdated information from powering AI answers |

### Duplicate Detection

| Capability | Description |
|---|---|
| AI-powered detection | Flags content with 90%+ similarity as potentially duplicative |
| Weekly scans | Runs automatically every Monday for teams with 100+ published cards |
| Merge suggestions | Recommends merging or consolidating redundant content |
| Single source of truth | Helps maintain clean, non-redundant knowledge base |

---

## 5. AI Knowledge Agents

### Core Capabilities

| Capability | Description |
|---|---|
| Instant answers | Delivers fast, cited AI responses across all platforms (web, Slack, Teams, browser extension) |
| Conversational chat | Follow-up questions to refine and explore topics deeper |
| Research Mode | Generates structured, multi-source reports for complex questions |
| Cross-document synthesis | Synthesizes insights across multiple documents and formats |
| Answer details | Shows exact source documents and sections used to generate each answer |
| Citation & explainability | Every answer includes citations, context, and reasoning steps |

### Agent Configuration

| Capability | Description |
|---|---|
| Custom agents | Create role-specific agents (IT, HR, Sales, Marketing, Support, Product) |
| Agent personalization | Customizable name, avatar, scope, and prompt settings per agent |
| Source scoping | Connect agents to all or a curated set of knowledge sources |
| Multi-source support | Connect to Guru Cards, Google Drive, SharePoint, Confluence, Notion, Slack, public websites |
| Admin-controlled | Admins manage source connections, access locations, and user roles |

### Agent Center

| Capability | Description |
|---|---|
| Centralized monitoring | All Knowledge Agent questions and answers in one dashboard |
| Knowledge gap identification | AI identifies unanswered questions and missing content areas |
| Usage analytics | Track question volume, answer quality, and performance metrics |
| Feedback loop | Employees flag incorrect or confusing answers; admins refine sources |
| Continuous improvement | Usage data feeds back into automatic improvement |
| Audit trail | Fully auditable and transparent interaction logging |
| Trend analysis | Query Agent Center data in chat or research mode to understand trends |
| Automation ROI | Measure time saved and automation return on investment |

### Distribution Channels

| Channel | Description |
|---|---|
| Guru web app | Central hub for search, chat, and research |
| Browser extension | Chrome, Edge, Opera — search and chat without tab switching |
| Slack | Answer questions in channels and DMs; proactive thread responses |
| Microsoft Teams | Search and receive announcements within Teams |
| MCP-enabled tools | Connect to Claude, ChatGPT, Cursor, and other AI tools |
| Custom apps | Embed via API into internal portals and custom applications |

---

## 6. AI Search & Discovery

### Agentic Search

| Capability | Description |
|---|---|
| Semantic search | Understands meaning behind queries, not just keyword matching |
| Cross-app search | Search across Guru and all connected external tools simultaneously |
| Federated search (2026) | Real-time queries across Google Drive, Box, SharePoint, Confluence without importing/indexing content |
| Web search | Real-time federated queries to publicly available web pages |
| Role-aware results | Results filtered based on user's role and permissions |
| Contextual relevance | AI considers user role, workflow, and query patterns for ranking |
| Suggested answers | Proactive answer suggestions before completing a search |
| Full-text search | Search across all content fields, titles, and metadata |
| AI-recommended experts | Automated expert matching when no document answer exists |

### Search Features

| Capability | Description |
|---|---|
| Instant results | Real-time results as you type |
| Source attribution | Every result linked to its source document |
| Filters | Filter by Collection, author, verification status, date |
| Search history | Track previous searches |
| Popular content | Surface frequently accessed knowledge |

---

## 7. AI Content Assist

| Capability | Description |
|---|---|
| AI writing assistant | Generative AI for creating and refining content |
| Content drafting | Generate first drafts from prompts or outlines |
| Content refinement | Improve grammar, clarity, tone, and structure |
| Tone adjustment | Rewrite content for different audiences |
| Summarization | Condense long-form content into key points |
| Translation | Translate content into other languages |
| Custom assist actions | Admins can customize, add, or remove Assist menu actions |
| Chat-to-card | Convert Knowledge Agent chat responses into published Guru Cards |

---

## 8. Intranet & Company Hub

### Custom Pages

| Capability | Description |
|---|---|
| Fully editable pages | AI-powered landing pages for teams, departments, or topics |
| Company homepage | Central organizational homepage with curated content |
| Department hubs | Dedicated pages per team with relevant cards and resources |
| Project pages | Topic-specific or project-specific knowledge hubs |
| Built-in access controls | Restrict page visibility by group or role |
| Embedded content | Embed cards, links, media, and dynamic content on pages |
| Page analytics | Track views, engagement, and user activity per page |

### Employee Directory

| Capability | Description |
|---|---|
| Employee profiles | View role, department, location, manager, and contact info |
| Org chart | Visual hierarchical reporting structure |
| HRIS sync | Auto-update profiles from BambooHR, Workday, Gusto (within 24 hours) |
| People discovery | Find colleagues by name, role, department, or expertise |
| Manager relationships | Visualize who reports to whom across the organization |

### Team Hubs

| Capability | Description |
|---|---|
| Dedicated home bases | Go-to pages for critical content and institutional knowledge |
| Curated content | Highlight key cards, links, and resources per team |
| Quick navigation | Fast access to team-specific collections and folders |

---

## 9. Contextual Knowledge Delivery

### Browser Extension

| Capability | Description |
|---|---|
| In-browser access | Access Guru from any webpage without switching tabs |
| Supported browsers | Chrome, Edge, Opera |
| Quick search | Search the knowledge base from any page |
| Card creation | Create new cards directly from the extension |
| Knowledge capture | Capture information from web pages into Guru |
| AI chat | Chat with Knowledge Agents from the extension |

### Knowledge Triggers

| Capability | Description |
|---|---|
| Contextual surfacing | Relevant cards appear automatically based on the website or tool being viewed |
| URL-based triggers | Configure triggers to activate on specific URLs or URL patterns |
| Proactive delivery | Information surfaces before the user searches — just-in-time knowledge |
| Workflow integration | Triggers activate within CRM, support tools, and internal apps |
| Reduced context switching | Eliminates need to leave the current tool to find answers |

### Suggested Answers (Slack)

| Capability | Description |
|---|---|
| Proactive thread answers | When a question is asked in a Slack channel, Guru's Knowledge Agent auto-responds in the thread |
| Trending topics | Detects trending questions and surfaces relevant knowledge |
| Channel-based | Enable suggested answers on specific Slack channels |

---

## 10. Announcements & Communication

| Capability | Description |
|---|---|
| Targeted announcements | Push time-sensitive updates to teams, individuals, or company-wide |
| Read tracking | Track exactly who has read each announcement and who hasn't |
| Comprehension confirmation | Optionally require users to acknowledge or confirm understanding |
| Page embedding | Display announcements on Custom Pages |
| Multi-channel delivery | Announcements visible in Guru app, Slack, and Teams |
| Time-sensitive updates | Share reminders, policy changes, and company-wide news |

---

## 11. Collaboration

| Capability | Description |
|---|---|
| Real-time co-editing | Multiple authors working on the same card simultaneously |
| Inline comments | Leave targeted comments on specific text within a card |
| Card-level comments | General discussion comments on any card |
| @mentions | Tag team members to notify and request input |
| Card sharing | Share individual cards via link or within Slack/Teams |
| Favorites | Bookmark frequently referenced cards |
| Reaction/feedback | Flag answers as helpful or inaccurate |
| Change history | View full edit history of any card |

---

## 12. Publishing Workflows

| Capability | Description |
|---|---|
| Approval-based publishing | Admins designate who can publish to each Collection |
| Draft mode | Authors without publish permission create drafts requiring approval |
| Review process | Content flows through a clear review pipeline before going live |
| Collection-level control | Publishing permissions set per Collection by Admins and Workspace Owners |
| Quality gates | Ensures consistent, accurate documentation — critical for regulated environments |

---

## 13. Analytics & Insights

### Dashboard Categories

| Dashboard | Description |
|---|---|
| Knowledge Health | Verification scores, stale content, content coverage |
| Performance | Most-viewed content, card engagement, adoption trends |
| Guru Impact | Team-level engagement, search success rates, time savings |

### Content Analytics

| Capability | Description |
|---|---|
| Card views | Track views per card and over time |
| Content usage patterns | Identify most-used and unused content |
| Verification score tracking | Monitor verification health across the knowledge base |
| Adoption trends | Measure knowledge base adoption over time |
| Content freshness | Identify stale or outdated content needing review |

### Page Analytics

| Capability | Description |
|---|---|
| Page views | Total views by Custom Page |
| Per-user views | Pivot table showing views by individual user |
| Engagement assessment | Assess page helpfulness and identify improvement areas |
| Time-based filtering | Filter views by time period |

### AI Answers Analytics

| Capability | Description |
|---|---|
| Question tracking | Monitor all questions asked to Knowledge Agents |
| Answer performance | Track answer quality and accuracy |
| Status filtering | Filter by status, assigned expert, and user feedback |
| AI vs. human performance | Compare AI-generated vs. human-curated answer effectiveness |
| Knowledge gap identification | Discover unanswered questions to prioritize content creation |

### Search Analytics

| Capability | Description |
|---|---|
| Search success rate | Percentage of searches that find relevant results |
| Failed searches | Identify queries with no or poor results |
| Search trends | Track most common search queries over time |
| Query patterns | Understand how users search for information |

### Export & Integration

| Capability | Description |
|---|---|
| Table export | Export analytics tables as CSV/spreadsheet |
| Scheduled reports | Set up recurring report delivery |
| API access | Pull analytics into BI tools (Tableau, Looker, Airtable) |
| Role-based access | Only Admins, Workspace Owners, and Authors can view analytics |

---

## 14. Permissions & Access Control

### Role-Based Access Control (RBAC)

| Role | Scope | Description |
|---|---|---|
| Admin | Workspace-wide | Full access to manage users, settings, and content across the entire workspace |
| Creator | Workspace-wide | Can create new objects (Collections, Knowledge Agents) but has no additional permissions unless they become the owner |
| Owner | Object-level | Edit, share, delete, and view analytics for specific assigned objects |
| Viewer | Object-level | View-only access to content without modification ability |
| Custom Roles | Configurable | Tailored sets of permissions across Guru objects, created by Admins |

### Permission Levels

| Level | Description |
|---|---|
| Collection-level | Control who can view, edit, and publish within each Collection |
| Folder-level | Share specific folders with groups — even without full Collection access |
| Card-level | Inherited from Collection/folder; can be further restricted |
| Knowledge Agent-level | Control who can access and use specific agents |
| Page-level | Custom Pages have built-in access controls |

### Access Management

| Capability | Description |
|---|---|
| Group-based permissions | Apply roles to multiple users at once via groups |
| Individual permissions | Assign permissions directly to specific users |
| Permission inheritance | Subfolders inherit parent folder permissions |
| AI permission awareness | Knowledge Agents respect real-time source system permissions for every answer |
| Source system permissions | Federated search honors permissions from connected tools (Google Drive, SharePoint, etc.) |

---

## 15. Integrations

### Communication & Collaboration

| Integration | Capabilities |
|---|---|
| **Slack** | Search, share, create cards, suggested answers in threads, announcements, Knowledge Agent responses, trending topics |
| **Microsoft Teams** | Search, announcements, Knowledge Agent access |

### Knowledge & Document Sources

| Integration | Capabilities |
|---|---|
| **Google Drive** | Federated search, content sync, Knowledge Agent source |
| **SharePoint** | Federated search, content sync, Knowledge Agent source |
| **Confluence** | Content sync, federated search, migration support |
| **Notion** | Content sync, Knowledge Agent source |
| **Box** | Federated search |
| **Dropbox** | Content sync |

### CRM & Sales

| Integration | Capabilities |
|---|---|
| **Salesforce** | Contextual knowledge triggers, in-app search |
| **HubSpot** | Knowledge triggers and contextual delivery |
| **Gong** | Sales call insights integration |

### Customer Support

| Integration | Capabilities |
|---|---|
| **Zendesk** | Agent-facing knowledge triggers, contextual card surfacing |
| **Intercom** | Knowledge delivery within support conversations |
| **Freshdesk** | Support agent knowledge access |
| **Front** | Knowledge within shared inboxes |
| **Kustomer** | Support workflow integration |
| **Gladly** | Customer service knowledge delivery |
| **Zoho Desk** | Support knowledge integration |

### Project Management

| Integration | Capabilities |
|---|---|
| **Asana** | Task and project knowledge linking |
| **Monday.com** | Workflow knowledge integration |
| **ClickUp** | Project knowledge connection |

### HR Systems (HRIS)

| Integration | Capabilities |
|---|---|
| **BambooHR** | Employee profile auto-sync |
| **Workday** | Employee data synchronization |
| **Gusto** | Employee information sync |

### AI & Developer Tools

| Integration | Capabilities |
|---|---|
| **ChatGPT** | Knowledge Agent access via MCP |
| **Claude** | Knowledge Agent access via MCP |
| **Cursor** | Developer knowledge via MCP |
| **OpenAI Agent Builder** | Connect Guru as a tool for custom AI agents |

### Total Integration Count

100+ out-of-the-box integrations across communication, document management, CRM, support, project management, HR, and AI platforms.

---

## 16. Security & Compliance

### Certifications & Standards

| Certification | Status |
|---|---|
| SOC 2 Type II | Certified (annual third-party audit covering Common Criteria, Confidentiality, Privacy) |
| GDPR | Compliant (EU standard contractual clauses, subprocessor agreements) |

### Data Protection

| Capability | Description |
|---|---|
| Encryption at rest | All stored data encrypted |
| Encryption in transit | All data encrypted during transmission (TLS) |
| Zero data retention | Third-party LLMs do not retain customer data after processing |
| Private AI model | Guru uses a private AI model; user data never trains AI models |
| Data masking (DLP) | Data Loss Prevention capabilities to protect sensitive information |
| PII protection | Personal information protected unless explicitly included in content |
| AWS hosting | Content stored in secure AWS infrastructure, separated by unique team ID |

### Access Security

| Capability | Description |
|---|---|
| SAML-based SSO | Single sign-on via identity providers (Okta, Azure AD, OneLogin, etc.) |
| SCIM provisioning | Automatic user account provisioning and de-provisioning |
| IP whitelisting | Restrict access to specific IP addresses |
| Role-based access control | Granular permissions at workspace, collection, folder, and object levels |
| Staff access controls | Guru staff do not have access to client data by default; requires CTO approval |
| Access reviews | Staff access reviewed three times annually |

### Development Security

| Capability | Description |
|---|---|
| Secure coding | Security baked into the development process |
| Developer security training | Specialized training for common vulnerabilities (XSS, SQL injection, etc.) |
| Vulnerability management | Ongoing vulnerability scanning and remediation |

---

## 17. Administration

### User Management

| Capability | Description |
|---|---|
| User provisioning | Add users manually, via SSO, or via SCIM auto-provisioning |
| User de-provisioning | Remove or deactivate users; SCIM auto-deprovisioning |
| Group management | Create and manage user groups for permission assignment |
| Role assignment | Assign built-in or custom roles to users and groups |
| HRIS sync | Auto-sync employee profiles, titles, managers, and locations |

### Content Management

| Capability | Description |
|---|---|
| Collection management | Create, configure, and archive Collections |
| Template management | Create, edit, and manage content templates |
| Publishing controls | Set publishing permissions per Collection |
| Verification settings | Configure default verification intervals and verifiers |
| Duplicate management | Review and resolve flagged duplicates |
| Content quality rules | Define rules for Knowledge Quality Automation |

### Agent Management

| Capability | Description |
|---|---|
| Agent creation | Create and configure Knowledge Agents |
| Source management | Connect and scope knowledge sources per agent |
| Prompt customization | Customize agent prompts and behavior |
| Assist action management | Add, remove, or customize Content Assist menu actions |
| Agent monitoring | Track agent performance via Agent Center |

### Workspace Settings

| Capability | Description |
|---|---|
| SSO configuration | Set up SAML SSO with identity provider |
| SCIM configuration | Configure automatic user provisioning |
| Security policies | Set IP whitelisting, DLP, and access policies |
| Branding | Customize workspace appearance |
| Integration management | Connect, configure, and manage third-party integrations |

---

## 18. API & Extensibility

### Guru API

| Capability | Description |
|---|---|
| REST API | Programmatic access to cards, collections, search, users, and analytics |
| Search API | Native search with personalized AI answers |
| Analytics API | Pull analytics data into external BI tools |
| Card CRUD | Create, read, update, and delete cards programmatically |
| Collection management | Manage collections and folders via API |
| User management | Manage users and groups programmatically |

### MCP Server (Model Context Protocol)

| Capability | Description |
|---|---|
| Open standard | Connects Knowledge Agents to external AI systems using MCP |
| Permission-aware | External AI tools access only governed, permission-aware knowledge |
| Audit integration | All MCP interactions flow into Agent Center for audit and monitoring |
| Supported tools | Claude, ChatGPT, Cursor, OpenAI Agent Builder, and any MCP-compatible client |
| Lineage tracking | Full tracking of how knowledge is used across external AI systems |
| No raw data access | External tools rely on Guru's cited knowledge layer, not raw documents |

### Custom Integrations

| Capability | Description |
|---|---|
| Webhook support | Event-driven notifications for external systems |
| Embed via API | Embed Guru search and answers into internal portals and custom apps |
| Custom knowledge triggers | Configure URL-based triggers for contextual knowledge delivery |

---

## 19. Pricing & Plans

### Plan Tiers

| Plan | Price | Description |
|---|---|---|
| Starter | $25/seat/month (annual) / $30/seat/month (monthly) | Predictable pricing for teams getting started |
| Enterprise | Custom pricing | Usage-based pricing built for scale and governance |

### Features Included in All Plans

| Feature | Included |
|---|---|
| Custom AI Knowledge Agents | Yes |
| Knowledge Quality Automation | Yes |
| Enterprise AI Search | Yes |
| AI Chat & Research | Yes |
| Verified Knowledge Base | Yes |
| Advanced Governance Controls | Yes |
| MCP Server | Yes |
| 100+ Integrations | Yes |
| SOC 2 Type 2 & GDPR | Yes |
| Data encryption (rest + transit) | Yes |
| Role-based access control | Yes |
| SAML-based SSO & SCIM | Yes |
| IP Whitelisting | Yes |
| Zero data retention (LLMs) | Yes |
| Private AI model | Yes |

### AI Credits

- Included in all plans with usage limits
- Each credit represents automated work
- Different AI capabilities consume credits based on complexity
- Predictable scaling without surprise costs

---

## 20. Summary Metrics

| Metric | Value |
|---|---|
| Content unit | Knowledge Cards |
| Organization levels | 3 (Collections → Folders → Cards) |
| Folder nesting depth | Up to 3 levels |
| Built-in roles | 5 (Admin, Creator, Owner, Viewer, Custom) |
| AI capabilities | 15+ (Agents, Agentic Search, Research Mode, Content Assist, Quality Automation, Duplicate Detection, etc.) |
| Integrations | 100+ out-of-the-box |
| Communication integrations | Slack, Microsoft Teams |
| CRM integrations | Salesforce, HubSpot, Gong |
| Support integrations | Zendesk, Intercom, Freshdesk, Front, Kustomer, Gladly, Zoho Desk |
| Document source integrations | Google Drive, SharePoint, Confluence, Notion, Box, Dropbox |
| HRIS integrations | BambooHR, Workday, Gusto |
| AI tool integrations | ChatGPT, Claude, Cursor (via MCP) |
| Browser extension support | Chrome, Edge, Opera |
| Mobile support | Web-based (no native mobile app) |
| Security certifications | SOC 2 Type II, GDPR |
| SSO support | SAML-based (Okta, Azure AD, OneLogin, etc.) |
| Deployment model | Cloud (SaaS) only |
| Plan tiers | 2 (Starter, Enterprise) |
| Duplicate detection threshold | 90% similarity |
| Duplicate scan frequency | Weekly (Monday, for 100+ card teams) |
| HRIS sync frequency | Within 24 hours of source change |
| Analytics dashboards | 3 categories (Knowledge Health, Performance, Guru Impact) |

---

## Sources

- [Guru Features](https://www.getguru.com/features)
- [Guru Pricing](https://www.getguru.com/pricing)
- [Guru Knowledge Agents](https://www.getguru.com/features/knowledge-agents)
- [Guru Analytics](https://www.getguru.com/features/analytics)
- [Guru Security](https://www.getguru.com/security)
- [Guru MCP Server](https://www.getguru.com/features/mcp-server)
- [Guru Content Templates](https://www.getguru.com/features/content-templates)
- [Guru Collaborative Editor](https://www.getguru.com/features/editor)
- [Guru Custom Pages](https://www.getguru.com/features/custom-pages)
- [Guru Collections & Folders](https://www.getguru.com/features/collections-folders)
- [Guru User Roles & Permissions](https://www.getguru.com/features/user-roles-permissions)
- [Guru Duplicate Detection](https://www.getguru.com/features/duplicate-detection)
- [Guru Slack Integration](https://www.getguru.com/integrations/slack)
- [Guru AI Intranet](https://www.getguru.com/solutions/employee-intranet)
- [Guru Help Center — Analytics](https://help.getguru.com/docs/utilizing-guru-analytics)
- [Guru Help Center — Permissions](https://help.getguru.com/docs/understanding-and-managing-permissions-in-guru)
- [Guru Help Center — Card Templates](https://help.getguru.com/docs/using-card-templates)
- [Guru Help Center — Org Chart & HRIS](https://help.getguru.com/docs/view-your-employee-org-chart-and-sync-from-your-hris)
- [Guru AI Platform Review (eesel.ai)](https://www.eesel.ai/blog/guru-ai)
- [Guru Review 2026 (siit.io)](https://www.siit.io/tools/trending/guru-review)
- [What Is Guru? (Bloomfire)](https://bloomfire.com/resources/what-is-guru/)
- [Guru AI 2026 Review (featurebase.app)](https://www.featurebase.app/blog/guru-ai)
