import { useState } from "react";
import ErrorList from "./ErrorList";
import { icons } from "../icons";

// category is null when adding, or the category being edited.
const CategoryForm = ({ category, onSaved, onCancel }) => {
    const isEditing = category !== null;

    const [name, setName] = useState(isEditing ? category.name : "");
    const [iconKey, setIconKey] = useState(isEditing ? category.iconKey : "");
    const [minWeight, setMinWeight] = useState(isEditing ? String(category.minWeightKg) : "");
    const [errors, setErrors] = useState([]);
    const [saving, setSaving] = useState(false);

    async function handleSubmit(event) {
        event.preventDefault();
        setErrors([]);

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

        const url = isEditing ? `/api/categories/${category.id}` : "/api/categories";
        const method = isEditing ? "PUT" : "POST";

        try {
            const response = await fetch(url, {
                method: method,
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request),
            });

            if (response.ok) {
                onSaved(isEditing ? "Category updated." : "Category added.");
                return;
            }

            const data = await response.json();
            setErrors(data.errors || ["Something went wrong."]);
        } catch {
            setErrors(["Could not reach the server. Is the API running?"]);
        }

        setSaving(false);
    }

    return (
        <div>
            <h2>{isEditing ? "Edit category" : "Add a category"}</h2>
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
                    {saving ? "Saving..." : isEditing ? "Save changes" : "Add category"}
                </button>{" "}
                <button type="button" onClick={onCancel}>
                    Cancel
                </button>
            </form>

            <ErrorList errors={errors} />
        </div>
    );
};

export default CategoryForm;