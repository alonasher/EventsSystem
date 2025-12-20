import { Link, useLocation } from "react-router-dom";
import "./navbar.css";

const Navbar = () => {
  const location = useLocation();

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link
          to="/"
          className={
            "navbar-link " +
            (location.pathname === "/" ? "navbar-link--active" : "")
          }
        >
          Recording
        </Link>
        <Link
          to="/analyze"
          className={
            "navbar-link " +
            (location.pathname === "/analyze" ? "navbar-link--active" : "")
          }
        >
          Analyze
        </Link>
      </div>
    </nav>
  );
};

export default Navbar;