Setup Requirements
- Ensure `DB_CONNECTION_STRING` is set for MySQL.
- Apply the additional DDL:
  - Add the unique constraint on `people.GMC`.
  - Ensure `specialties` and `people_specialties` tables exist.
  - Seed the initial specialties.
- Run `dotnet restore` and then `dotnet test`.

How to Use the Import Function
- Navigate to `/Import`.
- Upload a JSON file that follows the sample structure:
  - `firstName`, `lastName`
  - `gmc` as a 7-digit numeric string
  - Optional `address` array containing `line1`, `city`, `postcode`
- The validator checks:
  - GMC is exactly 7 digits
  - Duplicate GMCs within the file
  - Existing GMCs in the database (these are skipped)
- Database uniqueness also enforces GMC integrity.

Design and Implementation Notes
- Fully asynchronous pipeline for I/O.
- Validator operates purely in-memory for clarity and testability.
- Import is idempotent based on GMC.
- Database uniqueness ensures robustness.
- Repository layer handles all reads/writes cleanly.
- UI provides a summary and error list after each import.
- Specialties management and doctor–specialty assignment are exposed through the new CRUD pages and the doctor edit form.

If More Time Were Available
- Add a transaction around the entire import and provide true bulk-insert support for large files.
- Expand validation (e.g., postcode rules, mandatory address fields).
