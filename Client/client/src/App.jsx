import { Component } from "react";
import { useState } from "react";
import VehicleList from "./Components/VehicleList";
import VehicleForm from "./Components/VehicleForm";
import CategoryAdmin from "./Components/CategoryAdmin";
const App = () => {
  const [refreshKey, setRefreshKey] = useState(0);
  function handleVehicleAdded() {
    setRefreshKey(refreshKey + 1);
  }
  // Whenever a vehicle or a category changes, reloading vehicle list.
  function handleDataChanged() {
    setRefreshKey(refreshKey + 1);
  }
  return (
    <>
      <h1>Vehicle Management</h1>
      <VehicleForm onVehicleAdded={handleVehicleAdded} />
      <VehicleList refreshKey={refreshKey} />
      <CategoryAdmin onCategoriesChanged={handleDataChanged} />
    </>
  );
};

export default App;
