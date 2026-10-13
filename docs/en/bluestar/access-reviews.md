```json
//[doc-seo]
{
    "Description": "Bluestar Access Reviews: run access certification campaigns, assign multi-level reviewers, use risk signals, revoke access automatically and produce audit and compliance reports."
}
```

# Access Reviews

Access Reviews (also called **access certification**) let you check, at regular intervals, that people still need the access they hold. You set up a **review campaign** once. Bluestar then:

* captures the in-scope accounts and entitlements on schedule;
* sends them to the right reviewers;
* highlights risky access;
* removes revoked access automatically;
* produces an audit-ready report.

**In this document**

* [Key Concepts](#key-concepts)
* [Creating a Review Campaign](#creating-a-review-campaign)
* [Configuring a Review Campaign](#configuring-a-review-campaign)
* [Campaign Lifecycle](#campaign-lifecycle)
* [Reviewing Access (Reviewer Guide)](#reviewing-access-reviewer-guide)
* [Risk Signals](#risk-signals)
* [Remediation](#remediation)
* [Reports and Exports](#reports-and-exports)
* [Permissions](#permissions)
* [API Reference](#api-reference)
* [Good Practices and Things to Know](#good-practices-and-things-to-know)

## Key Concepts

| Concept | Description |
| --- | --- |
| **Review Campaign** | The definition of a review: what is reviewed (scope), how often (schedule), who reviews (reviewers), which accounts to include (settings), and what happens to revoked access (remediation). |
| **Run** | One execution of a campaign, identified by a run code such as `AR-20261013-001`. A recurring campaign creates a new run every time it is scheduled. |
| **Review item** | One account on an application, or one entitlement on an account, that a reviewer must decide on. Items are captured when the run starts, so later changes don't affect the run. |
| **Reviewer level** | One step of the reviewer chain. Level 1 reviews first; each later level reviews after every reviewer at the level before has submitted. |
| **Decision** | **Approve** (keep the access) or **Revoke** (remove it). Items left undecided past the deadline are **auto-revoked**. |

### Review types

| Type | Use it to… |
| --- | --- |
| **User Review** | Review all access held by selected users, organizations or groups. |
| **Application Review** | Review who has access to selected applications or entitlements. |

## Creating a Review Campaign

Go to **Review** and click **Create Review Campaign**.

| Field | Required | Description |
| --- | --- | --- |
| **Type** | Yes | **Application Review** (the default) or **User Review**. |
| **Name** | Yes | Up to 100 characters. It can't be only numbers, and it can't contain `< > : " / \ | ? * # % & { } [ ] ^ ~ `` ` ``. |
| **Description** | No | Up to 250 characters. |

Click **Save & Continue** to open the configuration page.

## Configuring a Review Campaign

The configuration page has five tabs, used in this order:

**Scope → Schedule → Reviewers → Settings → Remediation**

Each tab has **Save** and **Save & Next** buttons. The header has an **Enable Review / Disable Review** button, which becomes available once a schedule exists. A disabled review doesn't run on schedule and can't be started with **Start Now**.

### 1. Scope

**User Review: User Scope Selection**

* **All Users**: every active user.
* **Specific Users**: any combination of:
  * **Organizations** (*Add Organization*)
  * **Groups** (*Add Group*)
  * **Individuals** (*Add User*)

**Application Review: Application Scope Selection**

* **All Applications**: every active application.
* **Specific Applications**: any combination of:
  * **Applications** (*Add Application*)
  * **Entitlements** (*Add Entitlement*)

You need at least one selection.

### 2. Schedule

| Setting | Description |
| --- | --- |
| **Automatic Run** | Turns scheduled runs on or off. |
| **Frequency** | **One Time**, **Daily** (every *n* days or hours), **Weekly** (pick at least one day), or **Monthly** (pick months and dates 1–31; if you pick no months, every month is used). |
| **Start Date** | When the schedule starts. It uses your browser's time zone. |

The tab also shows **Last Run** and **Next Scheduled Run**. You can start a run at any time with **Start Now**.

### 3. Reviewers

Build the **approval chain** with **Add Level Approver**. Levels run in order, starting at Level 1.

| Setting | Description |
| --- | --- |
| **Configure Approver → Predefined** | **Manager**: the user's manager, from the *Manager* attribute on the user profile. **Application Owner**: the owner of the application, or of the entitlement's application. |
| **Configure Approver → Custom** | A specific **User**. |
| **Response Time** | Days the reviewer has to respond. The default is 3. The run's due date is the start date plus the response times of all levels added together. |
| **Enable Reminders** / **Reminder Interval** | Sends reminder emails every *n* days (the default is 1) until the reviewer responds. |
| **Enable Escalation** | After the deadline, notifies an **Escalation Person** and waits **Days to Wait After Escalation** (the default is 3) before the items are auto-revoked. |

You need at least one reviewer.

> **Important:** turn on **Enable Reminders** for every level. Reminders, escalation and the automatic revoke of unanswered items are all handled by the reminder process. If reminders are off for a level, its unanswered items stay pending.

> An item whose reviewer can't be worked out is not sent for review: for example, a *Manager* reviewer when the user has no manager recorded, or an *Application Owner* reviewer when the application has no owner. Make sure these attributes are filled in before you start a campaign.

### 4. Settings: Account Filter

These filters narrow down the accounts in the review. In each group, ticking **nothing** means *include all*.

| Filter | Options |
| --- | --- |
| **Account Types** | User Accounts, System Accounts, Service Accounts, Shared Accounts. |
| **Account Status** | Active, Disabled, Locked. |
| **Ownership Filters** | **Owner Assigned Only** or **Orphan Accounts Only**. Tick neither, or both, to include both. |
| **Entitlement Filter** | **High Risk** (critical permissions), **Medium Risk**, **Low Risk**. Only accounts that hold an entitlement of the ticked risk levels, and only those entitlements, are reviewed. |

Accounts and entitlements that are still waiting to be provisioned are never included.

### 5. Remediation

| Setting | Description |
| --- | --- |
| **Enable Auto Remediation** | Removes access automatically once it is revoked. It is on by default. See [Remediation](#remediation). |

## Campaign Lifecycle

```text
 Scheduled time / Start Now
          │
          ▼
 Run created (Upcoming → In Progress)
          │  snapshot of in-scope accounts & entitlements
          ▼
 Level 1 reviewers notified by email ──► reminders ──► deadline passed ──► escalation ──► auto-revoke
          │  all Level 1 reviewers submit
          ▼
 Level 2 … n reviewers (see previous level's decisions)
          │  final level submits
          ▼
 Run Completed ──► last-review details stamped on each account/entitlement
          ├──► Auto-remediation of revoked access
          └──► Audit Report (PDF) generated
```

### Run status

| Status | Meaning |
| --- | --- |
| **Upcoming** | Scheduled, not started yet. |
| **In Progress** | Waiting for reviewers. Shown on the **Active** tab. |
| **Overdue** | Still open after its due date. |
| **Completed** | All levels submitted. Also used when the scope matched no accounts; the run remarks explain this. |
| **Failed** | The run couldn't start, for example because no reviewers were configured. |

### Notifications

| Email | Sent to | When |
| --- | --- | --- |
| **Access Review Assigned** | Each reviewer, once per level | When the level starts. It includes the campaign, reviewer level, number of users to review, start and due dates, run code, and a link. |
| **Reminder** | Reviewers with pending items | Every reminder interval until the deadline. You get one email per reviewer, covering all their pending items. |
| **Escalated: Access Review Overdue** | Escalation Person | After the deadline, when escalation is on. It is for awareness only: the items stay with the original reviewer, and the email shows the date they will be auto-revoked. |
| **Expired: Access Review Request** | Reviewer | When their unanswered items are auto-revoked. |

> Outbound email must be turned on for your tenant. If it is off, reviewers still find their work in **My Workspace**.

### Unanswered items

When the deadline (the run's due date) has fully passed:

1. If escalation is on, the escalation person is notified, and the reviewer gets the extra waiting days.
2. After that, or straight away if escalation is off, the pending items are **auto-revoked**. The remark reads: *"No response received from the reviewer or the escalated contact within the allowed time."*
3. If auto-remediation is on, the revoked access is removed.

## Reviewing Access (Reviewer Guide)

Reviewers open their work from the **email link**, or from **My Workspace / My Tasks**. A reviewer can only open runs they have been assigned to.

### Run worklist

* **Tiles**: Total Review Items, Reviewed, Pending and Completion.
* **List**: users (in a User Review) or applications (in an Application Review), with their accounts, entitlements and status.
* **Actions**: **Review** opens the Detailed View; **Compact View** opens the Compact View.

### Compact View

The fastest way to decide many items at once.

* Choose **Users View** (filter by Organization), **Entitlements View** (filter by Risk Type) or **Applications View** (filter by Application Type).
* Pick an entry on the left, then decide each account or entitlement on the right: **Approve**, **Revoke**, or **Change** a decision.
* Add **Comments** (optional), and open **Review History** to see every level's reviewer, response, time and remark.

### Detailed View

* Step through items with **Previous / Next**, and filter by decision: *All / Pending / Approved / Revoked*.
* **Identity Insights** shows the person's role or title, location, employee duration, active and disabled accounts, and assigned and unassigned entitlements.
* Each row shows the account status (*Active / Disabled / Locked*) and how the access was assigned (*Direct Assignment / Self-service*).
* **Approver Progress** shows where each level stands.

### Bulk actions

| Level 1 | Level 2 and above |
| --- | --- |
| Approve All, Approve Remaining, Revoke All, Revoke Remaining, Reset All | **Accept All** (take the previous level's decisions), **Start Fresh**, **Revert All** |

### Save and submit

1. **Save** your decisions as often as you like.
2. **Submit Review** when every item is decided. You can't submit while items are pending ("You still have N pending review(s) to complete.") and you can submit only once.
3. When every reviewer at a level has submitted, the next level starts. After the final level, the run completes.

## Risk Signals

Bluestar flags risky access next to each review item, so reviewers can concentrate on what matters. Signals are calculated live when the reviewer opens the view, using the run's start date.

| Signal | Raised when | Default severity |
| --- | --- | --- |
| **Inactive / Disabled account** | The account is disabled, or hasn't been used for 90 days or more. Usage is checked where the application reports last login. | Medium |
| **Departed staff** | The user has left: marked as left, or their termination or last reporting date has passed, and the account is still enabled. | Critical |
| **Temporary / contract staff** | The user is non-staff, or their staff category suggests a temporary role (contractor, intern, consultant, vendor and so on). It warns within 30 days of the end date and goes up one level once the end date has passed. | Medium |
| **Transferred / seconded staff** | The user changed organization in the last 365 days. | Medium |
| **Incorrect department** | The access belongs to an organization the user is no longer in. | Medium |
| **Incorrect access rights** | The entitlement isn't part of the baseline provisioning policy for the user's organization and employee type. | High |
| **Unnecessary privileged access** | A privileged entitlement isn't part of the baseline. Shown as *Privileged access held* (Info) when no baseline exists. | Critical |
| **Shared / generic account** | The account is shared, service or system, or its name looks generic (admin, svc, helpdesk, guest and so on). Lowered when the account has a recorded owner. | High |
| **Obsolete test account** | The name looks like a test account (test, demo, uat, sandbox and so on) **and** it is disabled or hasn't been used for 180 days or more. | Medium |
| **Segregation of duties conflict** | The user's combined access breaks an active SoD rule. The conflicting pair is named. | From the SoD rule |

Severity levels are **Info, Low, Medium, High** and **Critical**. Items at **Medium or above** count as **Needs attention**. Info never does.

**What reviewers can do with signals**

* A summary bar above every list shows *Needs attention, Critical, High* and *Medium* counts.
* **Filter** by signal type, SoD conflicts, privileged accounts, or *No issues found*.
* **Sort** by *Highest risk, Most signals, Lowest risk* or *Default order*.
* Open the **Evidence** panel to see why a signal was raised and the **Recommended action**.

## Remediation

When **Enable Auto Remediation** is on, revoked items (including auto-revoked ones) are removed automatically when the run completes or the items expire:

| Revoked item | Action sent to the application |
| --- | --- |
| **Entitlement** | Remove Entitlement. |
| **Account** | **Disable** the account (it isn't deleted), and remove every entitlement it holds. |

Bluestar raises a single remediation request, without approval, and sends it to the application connectors. Items that have already been remediated are skipped. When the run completes, each reviewed account and entitlement is stamped with **Last Review Date**, **Decision** and **Reviewer**.

## Reports and Exports

### Review page

* **Reviews tab**: your campaigns.
  * Filter by Review Type, Frequency and Last Run (including *Never Run*).
  * Actions: **Start Now**, **Edit**, **Run History**.
* **Campaign tab**: **Active**, **Overdue** and **Completed** runs.
  * Filter by date, review type, status, **progress** (Not Started / In Progress / Nearly Done / Fully Decided) and **days overdue** (1–7, 8–30, more than 30).
* **Export**: downloads `Reviews.xlsx` with these columns: Review, Run, Review Type, Status, Review Period, Total Items, Reviewed, Pending, Completion %, Approved, Revoked, Expired, Days Overdue.
* **Run History**: every run of a campaign, with a status filter and a completion rate.

### Campaign Overview

An executive dashboard for each campaign.

* **Tiles**:
  * users, accounts and entitlements reviewed;
  * approved, revoked and pending items;
  * completion;
  * accounts in conflict.
* **Charts**: Decision Summary, Department Distribution, Risk & Compliance and Identity Breakdown.
  * *Risk & Compliance* covers dormant accounts, high-risk users and entitlements, orphan accounts, privileged users, policy violations and SoD conflicts.
* **Recent Activity**.
* **Recommendations**, for example:
  * overdue users;
  * more than 20 % of items pending;
  * more than 30 % of items revoked;
  * a backlog of high-risk items;
  * one department carrying most of the workload;
  * reviewers who haven't finished.

### Access Review Audit Report (PDF)

Generated automatically when a run completes. You can download it from the campaign (**Download Report**) or from **Reports → User Access Review Report**, and you can regenerate it. Each regeneration creates a new version.

It contains these sections:

* Campaign Information
* Reviewer Chain
* Executive Summary
* Risk Summary
* Key Observations
* Recommendations
* Decisions by Application
* Decisions by Department
* User Population
* Decisions
* Timeline
* Approver History
* Remediation
* Sign-off (Application Owner, Security Team, Compliance, Internal Audit)

The **Risk Score** is calculated as 40 × revoked % + 35 × high-risk % + 25 × pending %. It is rated **Critical** (70 or more), **High** (45 or more), **Moderate** (20 or more) or **Low** (below 20).

### Other reports

| Report | Description |
| --- | --- |
| **Application Access Review Report** | The review history for one application. Filter by campaign, run, user, department, organization, status, risk level, SoD-only, overdue-only and date range. Export to PDF or Excel. |
| **ISO 27001 A.9.2.5: Review of User Access Rights** | A compliance check of four things: campaigns are complete; every item is decided; rejected access has been remediated; held access is covered by a review. Export to PDF or Excel. |

## Permissions

| Permission | Allows |
| --- | --- |
| **Access Reviews** | View the review lists, runs, overview and reports. |
| ↳ Create / Edit / Delete | Create campaigns; configure them and use **Start Now**; delete campaigns. |
| **Access Review Campaigns → Create** | Open the *Create Review Campaign* page. |
| **Access Review User Scopes / Application Scopes / Runs / Snapshots** | Manage the matching parts of a campaign. |
| **Request Approvals → Edit** | **Reviewers** need this to save and submit decisions. |
| **Reports → Governance** | View and export the Application Access Review Report. |
| **Reports → Compliance** | View and export the ISO 27001 A.9.2.5 report. |

## API Reference

All endpoints need an authenticated user with the matching permission.

### Campaigns: `api/app/access-reviews`

| Method | Route | Purpose |
| --- | --- | --- |
| GET / POST | `/` | List or create campaigns. |
| GET / PUT / DELETE | `/{id}` | Read, update or delete a campaign. |
| PUT | `/Update-settings/{id}` | Save the account filter settings. |
| PUT | `/update-enableremediation/{id}` | Turn auto remediation on or off. |
| PUT | `/execute-access-review/{id}` | **Start Now**. |
| GET | `/active-reviews`, `/upcoming-reviews`, `/completed-reviews` | Run lists. |
| GET | `/{id}/overview` | Campaign Overview dashboard. |
| GET | `/{id}/approver-progress` | Progress for each reviewer level. |
| GET | `/get-user-compact-view/{runId}`, `/get-ent-compact-view/{runId}`, `/get-app-compact-view/{runId}` | Compact View data. |
| GET | `/{id}/audit-report`, `/{id}/audit-report/view` | Download or view the audit report. |
| POST | `/{id}/audit-report/regenerate` | Regenerate the audit report. |
| GET | `/download-token`, `/as-excel-file` | Export to Excel. The token is valid for 30 seconds. |

### Reviewer decisions: `api/app/request-approvals`

| Method | Route | Purpose |
| --- | --- | --- |
| PUT | `/access-review-Campaign-approval/{runId}` | Save decisions from the Compact View. |
| PUT | `/access-review-user-detailed-approval/{runId}` | Save decisions from the Detailed View (users). |
| PUT | `/access-review-application-detailed-approval/{runId}` | Save decisions from the Detailed View (applications). |
| PUT | `/access-review-Campaign-Approval-submit/{runId}` | Submit the review. |

### Scopes, runs and reports

| Route | Purpose |
| --- | --- |
| `api/app/access-review-user-scopes` | User scope (organizations, groups, users). |
| `api/app/access-review-application-scopes` | Application scope (applications, entitlements). |
| `api/app/access-review-runs` | Runs. |
| `api/app/access-review-snapshots` | Review items. |
| `api/app/reports/application-access-reviews` (`/pdf`, `/excel`) | Application Access Review Report. |
| `api/app/reports/iso27001/a925/access-review` (`/pdf`, `/excel`) | ISO 27001 A.9.2.5 report. |

## Good Practices and Things to Know

* **Fill in managers and application owners** before using the *Manager* or *Application Owner* reviewers. Items without a reviewer are not sent for review.
* **Turn on reminders for every level**, so overdue items are escalated and closed automatically.
* **Leave a filter group empty to include everything.** Use filters only to narrow the review.
* **Items are captured when the run starts.** Access granted later is reviewed in the next run.
* **Overdue is worked out live.** A run becomes overdue as soon as its due date has passed and it is still open.
* **Level 2 starts only when every Level 1 reviewer has submitted.**
* **Revoked accounts are disabled, not deleted**, so they can be restored if a decision was wrong.
* **Comments are optional**, but recommended for revocations. They appear in Review History and in the audit report.

## See Also

* [Lifecycle Workflows](workflows.md)
* [Bluestar Overview](index.md)
