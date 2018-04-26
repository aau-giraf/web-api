# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).

[//]: # ( ## [year]S[sprint-number]R[release-number] - yyyy-mm-dd)
[//]: # (Describe each version with the following sections: Added, Changed, Removed, Deprecated, Fixed, Security)

## Unreleased

### Added
- New class ActivityDTO which consists of an integer which indicates the order in relation to other activities and a pictogram
- Added year and week properties for weeks.
- All weeks implicitly exists and can be identified by year and week number.

### Changed
- UpdateWeek, DeleteWeek, and ReadUsersWeekSchedule now takes year and week number as parameters instead of an id.
- Changed access modifiers in DTO classes to better represent which properties should be set by the frontend.

### Removed
- Migrations are started over (all migrations are removed and a single new one is created instead)
- CreateWeek removed because all weeks implicitly exists and can be identified by year and week number.