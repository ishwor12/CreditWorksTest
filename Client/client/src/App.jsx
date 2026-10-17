import { Component } from "react";
import VehicleList from "./Components/VehicleList";
const App = () => {
  const [refreshKey, setRefreshKey] = useState(0);
  function handleVehicleAdded() {
    setRefreshKey(refreshKey + 1);
  }
  return (
    <>
      <h1>Vehicle Management</h1>
      <VehicleForm onVehicleAdded={handleVehicleAdded} />
      <VehicleList refreshKey={refreshKey} />
    </>
  );
};

export default App;
