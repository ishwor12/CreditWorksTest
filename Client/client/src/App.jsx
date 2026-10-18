import { useState } from "react";
import VehicleList from "./components/VehicleList";
import VehicleForm from "./components/VehicleForm";
import CategoryList from "./components/CategoryList";
import CategoryForm from "./components/CategoryForm";

const App = () => {
    const [activeTab, setActiveTab] = useState("vehicles");
    const [refreshKey, setRefreshKey] = useState(0);
    const [editingCategory, setEditingCategory] = useState(null);
    const [categoryMessage, setCategoryMessage] = useState("");

    // Whenever a vehicle or a category changes, the number goes up and the vehicle list reloads.
    function handleDataChanged() {
        setRefreshKey(refreshKey + 1);
    }

    // Clicking a tab button always starts fresh: no edit in progress and no old message.
    function openTab(tabName) {
        setActiveTab(tabName);
        setEditingCategory(null);
        setCategoryMessage("");
    }

    // The Edit button on a category opens the form tab with that category filled in.
    function handleEditCategory(category) {
        setEditingCategory(category);
        setCategoryMessage("");
        setActiveTab("addCategory");
    }

    // After a category is saved, go back to the list and show the message there.
    function handleCategorySaved(text) {
        setEditingCategory(null);
        setCategoryMessage(text);
        setActiveTab("categories");
        handleDataChanged();
    }

    // The selected tab gets the extra "active" style.
    function tabClass(tabName) {
        return activeTab === tabName ? "tab active" : "tab";
    }

    return (
        <div>
            <h1>Vehicle Management</h1>

            <div className="tabs">
                <button className={tabClass("vehicles")} onClick={() => openTab("vehicles")}>
                    Vehicles
                </button>
                <button className={tabClass("addVehicle")} onClick={() => openTab("addVehicle")}>
                    Add vehicle
                </button>
                <button className={tabClass("categories")} onClick={() => openTab("categories")}>
                    Categories
                </button>
                <button className={tabClass("addCategory")} onClick={() => openTab("addCategory")}>
                    {editingCategory ? "Edit category" : "Add category"}
                </button>
            </div>

            <div className="panel">
                {activeTab === "vehicles" && <VehicleList refreshKey={refreshKey} />}

                {activeTab === "addVehicle" && <VehicleForm onVehicleAdded={handleDataChanged} />}

                {activeTab === "categories" && (
                    <CategoryList
                        onEdit={handleEditCategory}
                        onCategoriesChanged={handleDataChanged}
                        initialMessage={categoryMessage}
                    />
                )}

                {activeTab === "addCategory" && (
                    <CategoryForm
                        category={editingCategory}
                        onSaved={handleCategorySaved}
                        onCancel={() => openTab("categories")}
                    />
                )}
            </div>
        </div>
    );
};

export default App;