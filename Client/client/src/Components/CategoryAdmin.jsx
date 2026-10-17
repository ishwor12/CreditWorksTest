import { useState, useEffect } from "react";
import CategoryIcon from "./CategoryIcon";
import ErrorList from "./ErrorList";
import { icons } from "../icons";

const CategoryAdmin = ({ onCategoriesChanged }) => {
    const [categories, setCategories] = useState([]);
    const [editingId, setEditingId] = useState(null);
    const [name, setName] = useState("");
    const [iconKey, setIconKey] = useState("");
    const [minWeight, setMinWeight] = useState("");
    const [errors, setErrors] = useState([]);
    const [message, setMessage] = useState("");
    const [saving, setSaving] = useState(false);
    const [reloadKey, setReloadKey] = useState(0);

    // Loads the categories when the page opens, and again whenever reloadKey changes.
    useEffect(() => {
        async function loadCategories() {
            try {
                const response = await fetch("/api/categories");

                if (response.ok) {
                    const data = await response.json();
                    setCategories(data);
                } else {
                    setErrors(["Could not load the categories."]);
                }
            } catch {
                setErrors(["Could not reach the server. Is the API running?"]);
            }
        }

        loadCategories();
    }, [reloadKey]);

    function clearForm() {
        setEditingId(null);
        setName("");
        setIconKey("");
        setMinWeight("");
    }

    // Called after a successful add, update or delete.
    function afterChange(text) {
        setMessage(text);
        clearForm();
        setReloadKey(reloadKey + 1);
        onCategoriesChanged();
    }

    function handleEdit(category) {
        setEditingId(category.id);
        setName(category.name);
        setIconKey(category.iconKey);
        setMinWeight(String(category.minWeightKg));
        setErrors([]);
        setMessage("");
    }

    async function handleSubmit(event) {
        event.preventDefault();
        setErrors([]);
        setMessage("");

        // An empty box would turn into 0, so it is checked here first.
        if (minWeight.trim() === "") {
            setErrors(["Minimum weight is required."]);
            return;
        }

        setSaving(true);

        const request = {
            name: name,
            iconKey: iconKey,
            minWeightKg: Number(minWeight),
        };

        const isEditing = editingId !== null;
        const url = isEditing ? `/api/categories/${editingId}` : "/api/categories";
        const method = isEditing ? "PUT" : "POST";

        try {
            const response = await fetch(url, {
                method: method,
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request),
            });

            if (response.ok) {
                afterChange(isEditing ? "Category updated." : "Category added.");
            } else {
                const data = await response.json();
                setErrors(data.errors || ["Something went wrong."]);
            }
        } catch {
            setErrors(["Could not reach the server. Is the API running?"]);
        }

        setSaving(false);
    }

    async function handleDelete(category) {
        const sure = window.confirm(`Delete the category "${category.name}"?`);
        if (!sure) {
            return;
        }

        setErrors([]);
        setMessage("");

        try {
            const response = await fetch(`/api/categories/${category.id}`, { method: "DELETE" });

            if (response.ok) {
                afterChange(`Deleted ${category.name}.`);
            } else {
                const data = await response.json();
                setErrors(data.errors || ["Something went wrong."]);
            }
        } catch {
            setErrors(["Could not reach the server. Is the API running?"]);
        }
    }

    function rangeText(category) {
        if (category.maxWeightKg === null) {
            return `${category.minWeightKg} kg and above`;
        }
        return `${category.minWeightKg} kg to under ${category.maxWeightKg} kg`;
    }

    return (
        <div>
            <h2>Categories</h2>
            <p>A weight equal to the end of a range belongs to the next category. For example, 500 kg is Medium.</p>

            <table>
                <thead>
                    <tr>
                        <th>Icon</th>
                        <th>Name</th>
                        <th>Weight range</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {categories.map((category) => (
                        <tr key={category.id}>
                            <td>
                                <CategoryIcon iconKey={category.iconKey} name={category.name} />
                            </td>
                            <td>{category.name}</td>
                            <td>{rangeText(category)}</td>
                            <td>
                                <button onClick={() => handleEdit(category)}>Edit</button>{" "}
                                <button onClick={() => handleDelete(category)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <h3>{editingId === null ? "Add a category" : "Edit category"}</h3>
            <p>You only enter where the range starts. It ends where the next category starts.</p>

            <form onSubmit={handleSubmit} noValidate>
                <div>
                    <label>Name </label>
                    <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
                </div>

                <div>
                    <label>Icon </label>
                    <select value={iconKey} onChange={(e) => setIconKey(e.target.value)}>
                        <option value="">-- choose --</option>
                        {Object.keys(icons).map((key) => (
                            <option key={key} value={key}>
                                {icons[key]} {key}
                            </option>
                        ))}
                    </select>
                </div>

                <div>
                    <label>Starts at (kg) </label>
                    <input type="number" step="0.01" value={minWeight} onChange={(e) => setMinWeight(e.target.value)} />
                </div>

                <button type="submit" disabled={saving}>
                    {saving ? "Saving..." : editingId === null ? "Add category" : "Save changes"}
                </button>{" "}
                {editingId !== null && (
                    <button type="button" onClick={clearForm}>
                        Cancel
                    </button>
                )}
            </form>

            <ErrorList errors={errors} />
            {message && <p style={{ color: "green" }}>{message}</p>}
        </div>
    );
};

export default CategoryAdmin;