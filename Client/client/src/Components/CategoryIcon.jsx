import { icons } from "../icons";

const CategoryIcon = ({ iconKey, name }) => {
    const picture = icons[iconKey] || "❓";

    return (
        <span title={name} role="img" aria-label={name} style={{ fontSize: "1.5rem" }}>
            {picture}
        </span>
    );
};

export default CategoryIcon;