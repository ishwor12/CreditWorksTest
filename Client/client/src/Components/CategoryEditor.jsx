import { useState, useEffect } from "react";
import CategoryIcon from "./CategoryIcon";
import ErrorList from "./ErrorList";
import { icons } from "../icons";
import { categoriesApi } from "../API/categoriesApi"; // <-- ADDED THIS

// Turns the categories from the API into table rows. The number boxes hold text.
function toRows(categories) {
  return categories.map((category) => ({
    id: category.id,
    name: category.name,
    iconKey: category.iconKey,
    min: String(category.minWeightKg),
    max: category.maxWeightKg === null ? "" : String(category.maxWeightKg),
  }));
}

const CategoryEditor = ({ onCategoriesChanged }) => {
  const [rows, setRows] = useState([]);
  const [errors, setErrors] = useState([]);
  const [message, setMessage] = useState("");
  const [saving, setSaving] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);

  // Loads the categories when the tab opens, and again whenever reloadKey changes.
  useEffect(() => {
    async function loadCategories() {
      try {
        const response = await categoriesApi.getAll(); // <-- CHANGED HERE

        if (response.ok) {
          setRows(toRows(response.data)); // <-- CHANGED HERE
        } else {
          setErrors(["Could not load the categories."]);
        }
      } catch {
        setErrors(["Could not reach the server. Is the API running?"]);
      }
    }

    loadCategories();
  }, [reloadKey]);

  // Changes one box in one row. The other rows stay as they are.
  function updateRow(index, field, value) {
    const newRows = [...rows];
    newRows[index] = { ...newRows[index], [field]: value };
    setRows(newRows);
  }

  function handleAddRow() {
    // id null tells the server this is a new category.
    setRows([...rows, { id: null, name: "", iconKey: "", min: "", max: "" }]);
    setMessage("");
  }

  function handleRemoveRow(index) {
    setRows(rows.filter((row, i) => i !== index));
    setMessage("");
  }

  // Throws away the changes and loads the saved categories again.
  function handleDiscard() {
    setErrors([]);
    setMessage("");
    setReloadKey(reloadKey + 1);
  }

  async function handleSave() {
    setErrors([]);
    setMessage("");

    // An empty "starts at" box would turn into 0, so it is checked here first.
    const missing = [];
    rows.forEach((row, index) => {
      if (row.min.trim() === "") {
        missing.push(`Row ${index + 1}: starts at is required.`);
      }
    });
    if (missing.length > 0) {
      setErrors(missing);
      return;
    }

    setSaving(true);

    const request = rows.map((row) => ({
      id: row.id,
      name: row.name,
      iconKey: row.iconKey,
      minWeightKg: Number(row.min),
      // An empty "ends before" box means no upper limit.
      maxWeightKg: row.max.trim() === "" ? null : Number(row.max),
    }));

    try {
      const response = await categoriesApi.updateAll(request); // <-- CHANGED HERE

      if (response.ok) {
        setRows(toRows(response.data)); // <-- CHANGED HERE
        setMessage("Categories saved.");
        onCategoriesChanged();
      } else {
        setErrors(
          Array.isArray(response.data.errors)
            ? response.data.errors
            : ["Something went wrong."],
        ); // <-- CHANGED HERE
      }
    } catch {
      setErrors(["Could not reach the server. Is the API running?"]);
    }

    setSaving(false);
  }

  return (
    <div>
      <h2>Categories</h2>
      <p>
        Each range includes its start and stops just before its end, so a weight
        equal to the end belongs to the next category. For example, 500 kg is
        Medium. Leave "Ends before" empty for the top category (no upper limit).
      </p>
      <p>Changes are not saved until you press "Save all changes".</p>

      <table>
        <thead>
          <tr>
            <th>Row</th>
            <th>Name</th>
            <th>Icon</th>
            <th>Starts at (kg)</th>
            <th>Ends before (kg)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={index}>
              <td>{index + 1}</td>
              <td>
                <input
                  className="name-input"
                  type="text"
                  value={row.name}
                  onChange={(e) => updateRow(index, "name", e.target.value)}
                />
              </td>
              <td>
                <CategoryIcon iconKey={row.iconKey} name={row.name} />{" "}
                <select
                  value={row.iconKey}
                  onChange={(e) => updateRow(index, "iconKey", e.target.value)}
                >
                  <option value="">-- choose --</option>
                  {Object.keys(icons).map((key) => (
                    <option key={key} value={key}>
                      {icons[key]} {key}
                    </option>
                  ))}
                </select>
              </td>
              <td>
                <input
                  className="range-input"
                  type="number"
                  step="0.01"
                  value={row.min}
                  onChange={(e) => updateRow(index, "min", e.target.value)}
                />
              </td>
              <td>
                <input
                  className="range-input"
                  type="number"
                  step="0.01"
                  placeholder="no limit"
                  value={row.max}
                  onChange={(e) => updateRow(index, "max", e.target.value)}
                />
              </td>
              <td>
                <button type="button" onClick={() => handleRemoveRow(index)}>
                  Remove
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <p>
        <button type="button" onClick={handleAddRow}>
          Add category
        </button>{" "}
        <button type="button" onClick={handleSave} disabled={saving}>
          {saving ? "Saving..." : "Save all changes"}
        </button>{" "}
        <button type="button" onClick={handleDiscard}>
          Discard changes
        </button>
      </p>

      <ErrorList errors={errors} />
      {message && <p style={{ color: "green" }}>{message}</p>}
    </div>
  );
};

export default CategoryEditor;
