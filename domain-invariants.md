# Domain Invariants

## Course cannot be shorter than one 1 hour, from 9:00 to 17:00
- **Description**: Course must be at least one day per week and more then 1 hour.
- **Tests**:
  - `TimeCourseTest.TimeSlot_Exceptions`.
  - `TimeCourseTest.PeriodTest_Constructor`.

## Courses should not overlap with other courses
- **Description**: course schedules should not overlap in time.
- **Tests**:
  - `TimeCourseTest.OverlapWithOtherTime`.

## Swagger must be accessible
- **Description**: Swagger UI (/swagger/index.html) must be accessible and return an HTTP 200 OK status to allow access to API documentation.
- **Tests**:
- `ApiSmokeTest.Swagger_Should_Return_OK`.  

  