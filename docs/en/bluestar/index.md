```json
//[doc-seo]
{
    "Description": "Bluestar identity governance documentation: lifecycle workflows and access reviews."
}
```

# Bluestar Documentation

Bluestar is an Identity Governance and Administration (IGA) platform. It automates the identity lifecycle and governs who has access to what across your connected applications.

## Guides

| Guide | What it covers |
| --- | --- |
| [Lifecycle Workflows](workflows.md) | Automating Joiner, Mover and Leaver processes: triggers, user scope, approvals, provisioning rules and run history. |
| [Access Reviews](access-reviews.md) | Access certification campaigns: scope, schedule, multi-level reviewers, risk signals, auto-remediation and audit reports. |

## How they fit together

```text
 HR / Trusted Source ──► Lifecycle Events ──► Lifecycle Workflows ──► Provisioning Requests ──► Applications
                                                                                    ▲
 Access Reviews ──► Reviewer decisions ──► Revoked access ──► Remediation Requests ─┘
```

* **Lifecycle Workflows** give people the right access when they join, move or leave.
* **Access Reviews** check regularly that the access is still appropriate, and remove what is not.
