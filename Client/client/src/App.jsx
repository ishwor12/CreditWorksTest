import { useState } from "react";
import VehicleList from "./components/VehicleList";
import VehicleForm from "./components/VehicleForm";
import CategoryEditor from "./components/CategoryEditor"; // Ensure this file exists

const App = () => {
  const [activeTab, setActiveTab] = useState("vehicles");
  const [refreshKey, setRefreshKey] = useState(0);

  // Whenever a vehicle or a category changes, the number goes up and the vehicle list reloads.
  function handleDataChanged() {
    setRefreshKey(refreshKey + 1);
  }

  // The selected tab gets the extra "active" style.
  function tabClass(tabName) {
    return activeTab === tabName ? "tab active" : "tab";
  }

  return (
    <div>
      <h1>Vehicle Management</h1>

      <div className="tabs">
        <button
          className={tabClass("vehicles")}
          onClick={() => setActiveTab("vehicles")}
        >
          Vehicles
        </button>
        <button
          className={tabClass("addVehicle")}
          onClick={() => setActiveTab("addVehicle")}
        >
          Add vehicle
        </button>
        <button
          className={tabClass("categories")}
          onClick={() => setActiveTab("categories")}
        >
          Categories
        </button>
      </div>

      <div className="panel">
        {activeTab === "vehicles" && <VehicleList refreshKey={refreshKey} />}
        {activeTab === "addVehicle" && (
          <VehicleForm onVehicleAdded={handleDataChanged} />
        )}
        {activeTab === "categories" && (
          <CategoryEditor onCategoriesChanged={handleDataChanged} />
        )}
      </div>
    </div>
  );
};

export default App;
