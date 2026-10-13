```json
//[doc-seo]
{
    "Description": "Bluestar Lifecycle Workflows: automate Joiner, Mover and Leaver identity processes with trigger conditions, user scope, approvals, provisioning policy rules and run history."
}
```

# Lifecycle Workflows

Lifecycle Workflows automate the **Joiner, Mover and Leaver (JML)** processes of your organization. When a person joins, changes role or leaves, Bluestar creates or updates their identity, raises a provisioning request, optionally routes it for approval, and sends the account and entitlement changes to the connected applications. You don't need to do any of these steps by hand.

**In this document**

* [Key Concepts](#key-concepts)
* [Creating a Workflow](#creating-a-workflow)
* [Configuring a Workflow](#configuring-a-workflow)
* [How a Workflow Runs](#how-a-workflow-runs)
* [Lifecycle Events](#lifecycle-events)
* [Monitoring and Run History](#monitoring-and-run-history)
* [Permissions](#permissions)
* [API Reference](#api-reference)
* [Good Practices and Things to Know](#good-practices-and-things-to-know)

## Key Concepts

| Concept | Description |
| --- | --- |
| **Lifecycle Workflow** | A reusable automation for one lifecycle phase (Joiner, Mover or Leaver). It defines *when* to act, *who* it applies to, *whether approval is needed* and *which provisioning tasks* run. A workflow does not hold users itself. |
| **Lifecycle Event** | One occurrence for one person, such as "Jane Doe joins on 1 November". An event carries the person's HR attributes and dates and is attached to a workflow. Events come from a **trusted-source import** (HR feed), the **contract-expiry** job, or are **entered manually**. |
| **Request** | The provisioning request that Bluestar creates when an event is processed. It contains one *request detail* per account or entitlement task. |
| **Run** | One execution of a workflow, either scheduled or started with **Run Now**. Each run picks up the events that are due and records the result in **Run History**. |

### Workflow types

| Type | Purpose |
| --- | --- |
| **Joiner** | New employee onboarding and access setup. |
| **Mover** | Role or department change management. |
| **Leaver** | Secure offboarding and access removal. |

> The type mainly describes the purpose of the workflow and groups the dashboard counts. The provisioning that actually happens is decided by the workflow's **Policy** rules and the **Provisioning Entitlement Policy**, described below.

### Event types

Each workflow can be labelled with an **Event Type**, taken from a configurable list. The default list is: *Onboarding, Rehire, Conversion, Transfer, Secondment - Start, Secondment - End, Long Leave - Start, Long Leave - Return, Termination*. The event type is shown for information only.

## Creating a Workflow

Go to **Automation → Lifecycle → Workflows** and click **Add Workflow**.

| Field | Required | Description |
| --- | --- | --- |
| **Select Workflow Event** | Yes | Choose **Joiner**, **Mover** or **Leaver**. The default is Joiner. |
| **Workflow Name** | Yes | Up to 100 characters. It can't be only numbers, and it can't contain `< > : " / \ | ? * # % & { } [ ] ^ ~ `` ` ``. |
| **Description** | No | Free text describing the purpose of the workflow. |
| **Event Type** | No | Choose a label from the event type list. |

Click **Create Workflow**. Bluestar opens the **configuration wizard** for the new workflow.

> **Tip:** Keep workflow names **unique**. HR (trusted-source) import files refer to a workflow by its name, and if two workflows share a name, the first match is used.

## Configuring a Workflow

The configuration wizard has five tabs, used in this order:

**Approval → Trigger Conditions → Scheduler Job → User Scope → Policy**

Each tab has **Save** and **Save & Next** buttons. The header shows the workflow name, its event type and an **Enable Workflow / Disable Workflow** button. That button becomes available once a schedule has been created on the Scheduler Job tab.

### 1. Approval

Choose whether the requests this workflow generates must be approved before anything is provisioned.

* **Require approval workflow**: when this is on, a request waits in *Pending Approval* and nothing is sent to the applications until it is approved. When it is off, requests are **auto-approved** and provisioned straight away.
* **Approval Chain**: add one or more levels with **Add Level Approver**. For each level, set:
  * **Approver**: a specific **User**, a **Role** or a **Group** (*Configure Approver → Predefined / Custom*).
  * **Response Settings**: use the default response time, or set a **Response Time in Days**.
  * **Reminders**: turn on **Send Reminders** and set the reminder interval.
  * **Escalation**: turn on **Enable Escalation**, choose an **Escalation Person**, and set how many days to wait before escalating.

> If approval is required but no approver is configured, **Run Now** is blocked, and the workflow list shows **Approver Not Configured**.

### 2. Trigger Conditions

Choose *when* the workflow acts, relative to a date on the person's event.

| Field | Values | Description |
| --- | --- | --- |
| **Time Relation** | Before / On / After | Whether to act before, on, or after the reference date. |
| **Number of Days** | 1 – 365 | How many days before or after the reference date. With **On**, this is set to 0 automatically. |
| **Reference Date** | Effective Date / Joining Date / Termination Date | The date on the event that the trigger is measured from. The default is **Effective Date**. |

**Examples**

* Joiner: *3 days **Before** the **Joining Date***, so accounts are ready before the first day.
* Leaver: ***On** the **Termination Date***.
* Leaver: *30 days **After** the **Termination Date***, for example to remove a mailbox that was retained.

> An event fires only on the **exact due date**. An event that has no value for the selected reference date (for example, no Termination Date) is never picked up by that workflow. All date calculations use the **UTC** date.

### 3. Scheduler Job

The scheduler decides how often Bluestar checks the workflow for due events.

| Setting | Description |
| --- | --- |
| **Automatic Sync** | Turns scheduled execution on or off. |
| **Run Every** | **One-time**, **Daily**, **Weekly** (choose days of the week) or **Monthly** (choose dates and months). |
| **Start Date** / **Recur Every** | When the schedule starts and how often it repeats. |

The tab also shows **Last Run** and **Next Scheduled Run**.

> A daily schedule is recommended. Events are matched on their exact due date, so a schedule that skips a day can miss events that fall due on that day.

### 4. User Scope

Choose which people the workflow applies to.

* **User Scope**
  * **All Users**: applies to everyone.
  * **Staff Only**: internal employees.
  * **Non-Staff Only**: external users, such as contractors.
* **Restrict to Organizations**: when this is on, the workflow applies only to people whose organization is in **Selected Organizations**. Use **Add Organization** to search and add organizations; each one shows its user count. You need at least one organization while this option is on.

> Organizations are matched **exactly** on the organization code of the event. Child organization units are **not** included automatically, so add each one you need.

### 5. Policy

The Policy tab controls *what* happens to the person's identity and accounts.

#### IAM User Action

This sets what happens to the person's Bluestar (IAM) user. It is saved as soon as you change it.

| Action | Effect |
| --- | --- |
| **Create if not exists** | Creates the IAM user if needed; otherwise updates the profile. The user is enabled. |
| **Update if exists** | Updates the profile and enables the user. The event fails if the user doesn't exist. |
| **Disable if exists** | Disables the user and marks them as having left. If no user exists, the event completes with nothing to do. |
| **Delete if exists** | Same as *Disable if exists*: users are **never hard-deleted**, so audit history is kept. |
| **No Action** | Leaves the IAM user as it is. The event fails if the user doesn't exist. |

When an IAM user is created by a workflow:

* it gets the default policy password and must change it at first login;
* it is assigned the **User** role;
* it is placed in the organization from the event.

#### Provisioning Rules

Rules decide which provisioning task runs for each account and entitlement, based on the item's current state and the state it should be in. A rule reads:

> **When** *Object Type* is in *Current State* **and** the *Required State* is … **then run** *Provisioning Task(s)*

| Object Type | Current State | Required State | Provisioning Tasks |
| --- | --- | --- | --- |
| **Account** | Account Does Not Exist, Account Exists, Account Is Disabled, Account Has Attribute Mismatch, Account Is Locked | Account Required, Account Not Required | Create Account, Update Account, Change Account Validity, Enable Account, Disable Account, Remove Account, Transfer Account, Reset Password, Unlock Account, Retain Account, No Action, *Activate Account / Update Account*, *Disable Account / Update Account* |
| **Entitlement** | Entitlement Does Not Exist, Entitlement Exists | Entitlement Required, Entitlement Not Required | Add Entitlement, Change Entitlement Validity, Remove Entitlement, Retain Entitlement, No Action |

* A rule needs at least one task. The combined options run both tasks.
* You can't add the same combination of object type, current state, required state and tasks twice.
* Use **Add Rule**, **Edit rule** and **Delete rule** to manage rules. Deleting a rule can't be undone.

**Example: Leaver workflow**

| When | And | Then run |
| --- | --- | --- |
| Account Exists | Account Not Required | Disable Account |
| Account Is Disabled | Account Not Required | Retain Account |
| Entitlement Exists | Entitlement Not Required | Remove Entitlement |

#### Which applications are provisioned?

You don't choose applications on the workflow. They come from the **Provisioning Entitlement Policy**, which grants applications and entitlements based on the person's **organization** and **employee type**. Bluestar compares what the policy grants with what the person already holds:

* **Required but missing**: handled by the *Required* rules, for example *Create Account* or *Add Entitlement*.
* **Held but no longer granted** (reconciliation): handled by the *Not Required* rules. This happens for every workflow type, so a Mover automatically loses access that belongs to their old role.
* **Already held and still granted**: kept as it is.

If a workflow has **no rules** for an object type, Bluestar uses sensible defaults:

* missing accounts are created, and disabled accounts are enabled;
* accounts that are no longer required are **disabled**;
* entitlements that are no longer required are **removed**.

Once you add rules for an object type, **only** your rules apply to it. A state that no rule covers is skipped.

## How a Workflow Runs

```text
Lifecycle Event (Pending)
        │  due date reached (scheduled run / Run Now / manual initiate)
        ▼
In Progress ──► IAM user synced (per IAM User Action)
        │
        ▼
Request created ──► Approval required? ──Yes──► Pending Approval ──► Approved ──┐
        │                                  No                                  │
        ▼                                                                      ▼
   Auto-approved ─────────────────────────────────────────────────► Sent to application connectors
                                                                               │
                                                                               ▼
                                              Event: Fulfillment In Progress → Completed / Partial / Failed
```

### Ways a workflow is triggered

1. **Scheduled run**: the scheduler job processes every event that is due today. Only one run of the same workflow can happen at a time; a run that overlaps another is skipped.
2. **Run Now**: from the workflow list. It processes the same due events as a scheduled run and doesn't bring future events forward. Progress appears in **Run History**. Run Now is blocked when:
   * the workflow is disabled;
   * approval is required but no approver is configured.
3. **Initiate a single event**: from the Events tab. This processes one event straight away, **ignoring its due date**. Use it to re-run a **Failed** event; it works on Pending, In Progress and Failed events.
4. **Trusted-source (HR) import**: each imported row names a workflow and an Effective Date.
   * Events due today are processed at once; the rest wait for the scheduled run.
   * A duplicate row (same Employee Code, workflow and Effective Date) is skipped.
5. **Contract-expiry offboarding**: a daily job targets one **Leaver** workflow.
   * It creates a Leaver event for each active **non-staff** user whose Termination Date is today, and processes it straight away.
   * A user who already has an event for that workflow and date is skipped.

### Which events a run picks up

An event is processed when **all** of these are true:

* it belongs to the workflow and its status is **Pending** (or **In Progress** from an interrupted run);
* the person matches the **User Scope** and, if it is turned on, the **organization** restriction;
* its reference date plus or minus the configured days equals **today (UTC)**.

## Lifecycle Events

Open **Automation → Lifecycle → Events** to see every event.

### Event statuses

| Status | Meaning |
| --- | --- |
| **Pending** | Waiting for its due date or for a run. |
| **In Progress** | Being processed by a run. |
| **Fulfillment In Progress** | The request was created and is waiting for approval or for the applications to respond. |
| **Completed** | All provisioning finished, or there was nothing to do. |
| **Partial** | Some provisioning actions failed or are still outstanding. |
| **Failed** | Processing failed, every application task failed, or the IAM User Action needed a user that doesn't exist. |
| **Cancelled** | Cancelled by an administrator. |

### Event actions

| Action | Description |
| --- | --- |
| **Add Event** | Creates an event for a **new user** (the Employee Code must be unique) or an **existing user**. |
| **Run Now / Initiate** | Processes the event straight away. |
| **Cancel Event / Reactivate Event** | Switches an event between Cancelled and Pending. Only allowed while the Effective Date is in the future. |
| **User Attributes** | Edits the person's attributes on the event. Only allowed while the Effective Date is in the future. |
| **Update Note** | Adds or changes a note on the event. |
| **View History** | Shows all events for the same person. |

The event detail page shows a **Workflow Process** timeline: *Event Created → Request Creation → Approval Status → Provisioning Process*.

## Monitoring and Run History

### Workflow list

* **Summary tiles**: Total, Joiner, Mover and Leaver counts.
* **Columns**: Name (with description), Applications, Approver and Type. *Approver* shows the approver names, **Auto Approved**, or **Approver Not Configured**.
* **Filter and search**: filter by Workflow Type, or search by name.
* **Row actions**: **Edit**, **Run Now** and **Run History**.
* **Export**: downloads `LifecycleWorkflows.xlsx` with Workflow, Description, Workflow Type, Applications, Approvers and Approval Required.

### Events list

* **Tiles**: Total Events, Completed, Pending and Failed.
* **Filters**:
  * Date period (Today, This Week, This Month, This Quarter, This Year, Last Week, Last Month, Last 3 Months);
  * Organization, Source Type, Workflow, Status.
* **Export**: downloads `LifecycleEvents.xlsx`.

### Run History

**Run History** lists the **30 most recent** runs of a workflow, scheduled and Run Now, newest first. It shows totals for Completed, Partial and Failed runs and a success rate.

| Run status | Meaning |
| --- | --- |
| **Queued** / **Running** | The run is waiting or in progress. The details page updates live. |
| **Completed** | No events failed or were left incomplete. |
| **Partial success** | Some events succeeded and some failed. |
| **Failed** | No events succeeded. |
| **Stopped** | The run ended without completing normally. |

A run with nothing to process is shown as **No events were due**.

Open a run to see **Run Details**:

* when it started and finished, its duration, and how many events it processed;
* a row per event, with its outcome and a link to the event;
* the **Run log**.

| Outcome | Typical result text |
| --- | --- |
| **Succeeded** | provisioning started |
| **Failed** | provisioning failed / failed - *reason* |
| **Skipped** | not due yet / already being processed by another run |
| **Incomplete** | event is no longer available |

## Permissions

| Permission | Allows |
| --- | --- |
| **Lifecycle Workflows** | View workflows, the summary, run history and exports. |
| ↳ Create / Edit / Delete | Add workflows; configure them (Approval, Trigger, User Scope, IAM Action) and use Run Now; delete workflows. |
| **Lifecycle Events** | View events, details and history, and initiate events. |
| ↳ Create / Edit / Delete | Add events; cancel or reactivate them, edit notes and attributes; delete events. |
| **Lifecycle Workflow User Scopes → Edit** | Edit a person's attributes on an event. |
| **Approval Chains** (+ Create / Edit / Delete) | Manage the Approval tab. |
| **Policies** (+ Create / Edit / Delete) | Manage provisioning rules on the Policy tab. |
| **Scheduler Jobs** (+ Edit) | Manage the Scheduler Job tab, Enable/Disable Workflow, and **Run Now**. |

The **Lifecycle** menu is shown to users who have *Lifecycle Workflows* or *Lifecycle Events*.

## API Reference

All endpoints need an authenticated user with the matching permission.

### Workflows: `api/app/lifecycle-workflows`

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/` | Paged list of workflows. |
| GET | `/lookup` | Id, name and description of each workflow. |
| GET | `/summary` | Total, Joiner, Mover and Leaver counts. |
| GET | `/overview` | List rows with application count and approvers. |
| GET | `/{id}` | Workflow details. |
| GET | `/lifecycle-workflow-user-scope/{id}` | User scope and selected organizations. |
| POST | `/` | Create a workflow. |
| PUT | `/{id}` | Full update. |
| PUT | `/approval/{id}` | Turn approval on or off. |
| PUT | `/trigger-condition/{id}` | Set the time relation, days and reference date. |
| PUT | `/user-scope/{id}` | Set the user scope and organizations. |
| PUT | `/iam-user-action/{id}` | Set the IAM User Action. |
| DELETE | `/{id}` | Delete a workflow. |
| GET | `/download-token`, `/as-excel-file` | Export to Excel. |

Updates need the latest `concurrencyStamp`. A stale value returns a conflict error.

### Events: `api/app/lifecycle-events`

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/overview`, `/summary`, `/details?id=` | List, counts and details. |
| GET | `/user-event-history?userId=` | All events for a person. |
| POST | `/create-event-for-new-user` | Create an event and a new user. |
| POST | `/create-event-for-existing-user` | Create an event for an existing user. |
| POST | `/initiate-event/{id}?isManualTrigger=true` | Process an event now. |
| PUT | `/set-event-cancellation-state/{id}` | Cancel or reactivate an event. |
| PUT | `/update-event-user-attributes/{id}` | Edit the person's attributes. |
| PUT | `/update-notes/{id}` | Update the note. |

### Other

| Method | Route | Purpose |
| --- | --- | --- |
| GET/POST/PUT/DELETE | `api/app/policies` | Manage provisioning rules. |
| POST | `api/job-scheduler/scheduler-jobs/trigger-by-workflow/{workflowId}` | Run Now. |
| GET | `api/app/job-runs?objectId={workflowId}&jobType=LifeCycle` | Run history. |

## Good Practices and Things to Know

* **Use unique workflow names**, because HR import files match workflows by name.
* **Schedule workflows daily**, because events fire only on their exact due date.
* **Add each organization you need** when restricting by organization; child units are not inherited.
* **Configure approvers before turning on approval**, or Run Now is blocked and requests wait without an approver.
* **Users are never deleted.** *Delete if exists* disables the user and keeps the audit history.
* **Re-run failed events** with **Initiate** on the Events tab. Scheduled runs don't retry Failed events.
* **The same person can have more than one event** for the same workflow, for example a rehire.
* **Dates are evaluated in UTC.**
* **Cancel or change events before their Effective Date.** After that date, an event can't be cancelled and its attributes can't be edited.

## See Also

* [Access Reviews](access-reviews.md)
* [Bluestar Overview](index.md)
