import { useState, useEffect } from "react";
import ErrorList from "./ErrorList";

const VehicleForm = ({ onVehicleAdded }) => {
  const [manufacturers, setManufacturers] = useState([]);
  const [ownerName, setOwnerName] = useState("");
  const [manufacturerId, setManufacturerId] = useState("");
  const [year, setYear] = useState("");
  const [weight, setWeight] = useState("");
  const [errors, setErrors] = useState([]);
  const [message, setMessage] = useState("");
  const [saving, setSaving] = useState(false);

  // Load the manufacturer list once, when the form first appears.
  useEffect(() => {
    async function loadManufacturers() {
      try {
        const response = await fetch("/api/manufacturers");

        if (response.ok) {
          const data = await response.json();
          setManufacturers(data);
        } else {
          setErrors(["Could not load the manufacturers."]);
        }
      } catch {
        setErrors(["Could not reach the server. Is the API running?"]);
      }
    }

    loadManufacturers();
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    setErrors([]);
    setMessage("");
    setSaving(true);

    // The inputs hold text, so the numbers are converted before sending.
    const newVehicle = {
      ownerName: ownerName,
      manufacturerId: Number(manufacturerId),
      yearOfManufacture: Number(year),
      weightKg: Number(weight),
    };

    try {
      const response = await fetch("/api/vehicles", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newVehicle),
      });

      const data = await response.json();

      if (response.ok) {
        setMessage(
          `Added ${data.ownerName}'s ${data.manufacturerName} as category ${data.categoryName}.`,
        );
        setOwnerName("");
        setManufacturerId("");
        setYear("");
        setWeight("");
        onVehicleAdded();
      } else {
        // The API sends its validation messages in an "errors" list.
        setErrors(data.errors || ["Something went wrong."]);
      }
    } catch {
      setErrors(["Could not reach the server. Is the API running?"]);
    }

    setSaving(false);
  }

  return (
    <div>
      <h2>Add a vehicle</h2>

      {/* noValidate: the browser does not check anything, so the server's messages are what you see. */}
      <form onSubmit={handleSubmit} noValidate>
        <div>
          <label>Owner's name </label>
          <input
            type="text"
            value={ownerName}
            onChange={(e) => setOwnerName(e.target.value)}
          />
        </div>

        <div>
          <label>Manufacturer </label>
          <select
            value={manufacturerId}
            onChange={(e) => setManufacturerId(e.target.value)}
          >
            <option value="">-- choose --</option>
            {manufacturers.map((m) => (
              <option key={m.id} value={m.id}>
                {m.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label>Year of manufacture </label>
          <input
            type="number"
            value={year}
            onChange={(e) => setYear(e.target.value)}
          />
        </div>

        <div>
          <label>Weight (kg) </label>
          <input
            type="number"
            step="0.01"
            value={weight}
            onChange={(e) => setWeight(e.target.value)}
          />
        </div>

        <button type="submit" disabled={saving}>
          {saving ? "Saving..." : "Add vehicle"}
        </button>
      </form>

      <ErrorList errors={errors} />
      {message && <p style={{ color: "green" }}>{message}</p>}
    </div>
  );
};

export default VehicleForm;
