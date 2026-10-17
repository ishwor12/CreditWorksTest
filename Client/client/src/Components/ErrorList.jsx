const ErrorList = ({ errors }) => {
  if (!errors || errors.length === 0) {
    return null;
  }

  return (
    <ul style={{ color: "red" }}>
      {errors.map((message, index) => (
        <li key={index}>{message}</li>
      ))}
    </ul>
  );
};

export default ErrorList;
