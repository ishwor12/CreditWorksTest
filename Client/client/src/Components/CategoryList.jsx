import { useState, useEffect } from "react";
import CategoryIcon from "./CategoryIcon";
import ErrorList from "./ErrorList";

const CategoryList = ({ onEdit, onCategoriesChanged, initialMessage }) => {
    const [categories, setCategories] = useState([]);
    const [errors, setErrors] = useState([]);
    const [message, setMessage] = useState(initialMessage || "");
    const [reloadKey, setReloadKey] = useState(0);

    // Loads the categories when the tab opens, and again whenever reloadKey changes.
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
                setMessage(`Deleted ${category.name}.`);
                setReloadKey(reloadKey + 1);
                onCategoriesChanged();
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
                                <button onClick={() => onEdit(category)}>Edit</button>{" "}
                                <button onClick={() => handleDelete(category)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <ErrorList errors={errors} />
            {message && <p style={{ color: "green" }}>{message}</p>}
        </div>
    );
};

export default CategoryList;