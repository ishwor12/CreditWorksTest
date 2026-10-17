import { useState, useEffect } from "react";
import CategoryIcon from "./CategoryIcon";

const headerStyle = { cursor: "pointer" };

const VehicleList = ({ refreshKey }) => {
  const [vehicles, setVehicles] = useState([]);
  const [sortBy, setSortBy] = useState("owner");
  const [descending, setDescending] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Runs when the page opens, and again every time the sort changes.
  useEffect(() => {
    async function loadVehicles() {
      setLoading(true);
      setError("");

      try {
        const response = await fetch(
          `/api/vehicles?sortBy=${sortBy}&descending=${descending}`,
        );

        if (response.ok) {
          const data = await response.json();
          setVehicles(data);
        } else {
          setError("Could not load the vehicles.");
        }
      } catch {
        setError("Could not reach the server. Is the API running?");
      }

      setLoading(false);
    }

    loadVehicles();
  }, [sortBy, descending, refreshKey]);

  function handleHeaderClick(column) {
    if (column === sortBy) {
      // Same column clicked again: flip the direction.
      setDescending(!descending);
    } else {
      setSortBy(column);
      setDescending(false);
    }
  }

  function arrowFor(column) {
    if (column !== sortBy) {
      return "";
    }
    return descending ? " ▼" : " ▲";
  }

  return (
    <div>
      <h2>Vehicles</h2>
      <p>
        Sorted by: {sortBy} ({descending ? "descending" : "ascending"}). Click a
        column title to change it.
      </p>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      <table>
        <thead>
          <tr>
            <th style={headerStyle} onClick={() => handleHeaderClick("owner")}>
              Owner{arrowFor("owner")}
            </th>
            <th
              style={headerStyle}
              onClick={() => handleHeaderClick("manufacturer")}
            >
              Manufacturer{arrowFor("manufacturer")}
            </th>
            <th style={headerStyle} onClick={() => handleHeaderClick("year")}>
              Year{arrowFor("year")}
            </th>
            <th style={headerStyle} onClick={() => handleHeaderClick("weight")}>
              Weight (kg){arrowFor("weight")}
            </th>
            <th>Category</th>
          </tr>
        </thead>
        <tbody>
          {vehicles.map((vehicle) => (
            <tr key={vehicle.id}>
              <td>{vehicle.ownerName}</td>
              <td>{vehicle.manufacturerName}</td>
              <td>{vehicle.yearOfManufacture}</td>
              <td>{vehicle.weightKg.toFixed(2)}</td>
              <td>
                <CategoryIcon
                  iconKey={vehicle.categoryIconKey}
                  name={vehicle.categoryName}
                />
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {!loading && !error && vehicles.length === 0 && <p>No vehicles yet.</p>}
    </div>
  );
};
export default VehicleList;
